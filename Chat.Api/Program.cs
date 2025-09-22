using Chat.Api.Hubs;
using Chat.Data;
using Chat.Domain.Interfaces;
using Chat.Services.Services;
using Chat.Services.Services.ServiceImpl;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configuración de appsettings.json + Environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Mongo settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// Repositorios y servicios
builder.Services.AddScoped<Chat.Domain.Interfaces.IMessageRepository, Chat.Data.Repositories.MessageRepository>();
builder.Services.AddScoped<Chat.Domain.Interfaces.IConversationRepository, Chat.Data.Repositories.ConversationRepository>();
builder.Services.AddSingleton<IMessageRepository, Chat.Data.Repositories.MessageRepository>();


builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMetaService, MetaService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddControllers();
builder.Services.AddSignalR(); // necesario
// Habilitar HttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7089") // ⚠️ Ajustá según tu Blazor WASM
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chathub");

app.Run();

