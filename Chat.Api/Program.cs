//using Chat.Api.Hubs;
//using Chat.Data;
//using Chat.Domain.Interfaces;
//using Chat.Services.Services;
//using Chat.Services.Services.ServiceImpl;
//using Microsoft.Extensions.Options;
//using MongoDB.Driver;

//var builder = WebApplication.CreateBuilder(args);
////builder.WebHost.UseUrls("http://0.0.0.0:5046");

//// Mongo settings
//builder.Services.Configure<MongoDbSettings>(
//    builder.Configuration.GetSection("MongoDbSettings"));

//builder.Services.AddSingleton<IMongoDatabase>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    var client = new MongoClient(settings.ConnectionString);
//    return client.GetDatabase(settings.DatabaseName);
//});

//// Repositorios y servicios
//// Program.cs (minimal API)
//builder.Services.AddScoped<Chat.Domain.Interfaces.IMessageRepository, Chat.Data.Repositories.MessageRepository>();
//builder.Services.AddScoped<Chat.Domain.Interfaces.IConversationRepository, Chat.Data.Repositories.ConversationRepository>();
//builder.Services.AddSingleton<IMessageRepository, Chat.Data.Repositories.MessageRepository>();

//builder.Services.AddScoped<IConversationService, ConversationService>();


//// Add services to the container.

//builder.Services.AddControllers();
//builder.Services.AddSignalR(); // <--- AGREGAR ESTO

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//// Habilitar HttpClientFactory
//builder.Services.AddHttpClient();
//var app = builder.Build();


//var serverAddresses = app.Urls; // esto ya tiene las urls configuradas
//Console.WriteLine("La API está corriendo en:");
//foreach (var address in serverAddresses)
//{
//    Console.WriteLine(address);
//}
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
//app.MapHub<ChatHub>("/chatHub");

//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
//app.Urls.Add($"http://*:{port}");

//app.Run();

using Chat.Data;
using Chat.Domain.Interfaces;
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

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(
        Environment.GetEnvironmentVariable("MONGO_CONNECTION")
        ?? settings.ConnectionString
    );
});

// Repositorios y servicios
builder.Services.AddScoped<Chat.Domain.Interfaces.IMessageRepository, Chat.Data.Repositories.MessageRepository>();
builder.Services.AddScoped<Chat.Domain.Interfaces.IConversationRepository, Chat.Data.Repositories.ConversationRepository>();
builder.Services.AddSingleton<IMessageRepository, Chat.Data.Repositories.MessageRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

