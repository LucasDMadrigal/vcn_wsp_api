using Chat.Domain.Entities;
using Chat.Services.Services;
using Chat.Shared.ApiMetaDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConversationService _conversationService;
        private readonly IConfiguration _configuration;

        public MessagesController(
            IHttpClientFactory httpClientFactory,
            IConversationService conversationService,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _conversationService = conversationService;
            _configuration = configuration;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] object body)
        {
            var jsonBody = body.ToString();
            var metaUrl = _configuration["Meta:BaseUrl"];
            var phoneNumberId = _configuration["Meta:PhoneNumberId"];
            var token = _configuration["Meta:AccessToken"];
            var url = metaUrl+"/"+phoneNumberId+"/messages";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonSerializer.Deserialize<MessageSentMetaResponseDto>(responseContent);
            var messageDto = JsonSerializer.Deserialize<MessageSentDto>(
                jsonBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // ignora mayúsculas/minúsculas
                }
            );
            var newMessageSent = new Message();

            newMessageSent.MetaMessageId = responseBody.Messages?.FirstOrDefault()?.Id;
            newMessageSent.From = "system";// Capturar el remitente de la respuesta
            newMessageSent.Type = messageDto.Type;
            newMessageSent.Text = messageDto.Text;
            newMessageSent.Template = messageDto.Template;
            newMessageSent.Direction = "outbound";
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.SentAt = DateTime.UtcNow;
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.To = responseBody.Contacts?.FirstOrDefault()?.Input ?? "UNKNOWN";
            newMessageSent.WaId = responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN";

            Console.WriteLine("Response content: " + jsonBody);
            //Console.WriteLine("Message sent: " +
            //    JsonSerializer.Serialize(newMessageSent, new JsonSerializerOptions { WriteIndented = true }));

            // Parsear el body para obtener el destinatario (to)
            using var doc = JsonDocument.Parse(jsonBody);
            var to = doc.RootElement.GetProperty("to").GetString();

            // Guardar en Mongo
            await _conversationService.AddMessageAsync(newMessageSent);



            return StatusCode((int)response.StatusCode, responseContent);
        }
    }
}
