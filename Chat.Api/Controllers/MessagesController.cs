using Chat.Domain.Entities;
using Chat.Services.Services;
using Chat.Shared.ApiMetaDTOs;
using Chat.Shared.DTOs;
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
        private readonly IMessageService _messageService;
        private readonly IConfiguration _configuration;

        public MessagesController(
            IHttpClientFactory httpClientFactory,
            IConversationService conversationService,
            IMessageService messageService,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _conversationService = conversationService;
            _messageService = messageService;
            _configuration = configuration;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(
            [FromBody] object body, 
            [FromHeader(Name = "Authorization")] string authorizationHeader,
            [FromHeader(Name = "meta-phone-number-id")] string metaPhoneNumberId,
            [FromHeader(Name = "X-User-Name")] string registeredUser
            )
        {
            var jsonBody = body.ToString();
            var metaUrl = _configuration["Meta:BaseUrl"];
            var phoneNumberId = metaPhoneNumberId;
            var token = authorizationHeader.Substring("Bearer ".Length);
            var user = registeredUser;
            var url = metaUrl+"/"+phoneNumberId+"/messages";

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return Unauthorized(new { Message = "Missing or invalid Authorization header" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content: " + responseContent);
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
            newMessageSent.From = string.IsNullOrEmpty(user) ? "System" : user;
            newMessageSent.Type = messageDto.Type;
            newMessageSent.Text = messageDto.Text;
            newMessageSent.Template = messageDto.Template;
            newMessageSent.Direction = "outbound";
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.SentAt = DateTime.UtcNow;
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.To = responseBody.Contacts?.FirstOrDefault()?.Input ?? "UNKNOWN";
            newMessageSent.WaId = responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN";

            //Console.WriteLine("Response content: " + jsonBody);
            
            using var doc = JsonDocument.Parse(jsonBody);
            var to = doc.RootElement.GetProperty("to").GetString();

            // Guardar en Mongo
            await _conversationService.AddMessageAsync(newMessageSent);



            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet ("{WaId}")]
        public async Task<IActionResult> GetMessagesByWaId([FromRoute] string WaId)
        {
            var messages = await _messageService.GetMessagesAsync(WaId);

            if (messages is null)
                return NotFound();

            var messagesDto = messages.Select(m => new MessageDto()
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                From = m.From,
                To = m.To,
                WaId = m.WaId,
                Type = m.Type,
                Text = m.Text,
                Template = m.Template,
                Direction = m.Direction,
                Status = m.Status,
                SentAt = m.SentAt,
                MetaMessageId = m.MetaMessageId
            }).ToList();

            return Ok(messagesDto);
            
        }
    }
}
