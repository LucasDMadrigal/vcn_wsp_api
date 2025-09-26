using Chat.Api.Hubs;
using Chat.Api.Mappers;
using Chat.Domain.Entities;
using Chat.Services.Services;
using Chat.Shared.ApiMetaDTOs;
using Chat.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ChatHub> _hubContext;
        public MessagesController(
            IHttpClientFactory httpClientFactory,
            IHubContext<ChatHub> hubContext,
            IConversationService conversationService,
            IMessageService messageService,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _conversationService = conversationService;
            _messageService = messageService;
            _configuration = configuration;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(
            //[FromBody] MessageDto messageDto, // 🚀 tipado directo
            [FromBody] object body, // 🚀 tipado directo
            [FromHeader(Name = "Authorization")] string authorizationHeader,
            [FromHeader(Name = "meta-phone-number-id")] string metaPhoneNumberId,
            [FromHeader(Name = "X-User-Name")] string registeredUser,
            [FromHeader(Name = "X-Client-Id")] string clientId
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

            Console.WriteLine("Response body: " + responseBody);
            var messageSentDto= JsonSerializer.Deserialize<MessageSentDto>(
                jsonBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // ignora mayúsculas/minúsculas
                }
            );
            var newMessageSent = new Message();

            newMessageSent.MetaMessageId = responseBody.Messages?.FirstOrDefault()?.Id;
            newMessageSent.From = string.IsNullOrEmpty(user) ? "System" : user;
            newMessageSent.ClientId = string.IsNullOrEmpty(clientId) ? "UNKNOWN" : clientId;
            newMessageSent.Type = messageSentDto.Type;
            newMessageSent.Text = messageSentDto.Text;
            newMessageSent.Template = messageSentDto.Template;
            newMessageSent.Direction = "outbound";
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.SentAt = DateTime.UtcNow;
            newMessageSent.Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN";
            newMessageSent.To = responseBody.Contacts?.FirstOrDefault()?.Input ?? "UNKNOWN";
            newMessageSent.WaId = responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN";

            var messageDto = new MessageDto()
            {
                Id = responseBody.Messages?.FirstOrDefault()?.Id,
                ConversationId = responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN",
                From = string.IsNullOrEmpty(user) ? "System" : user,
                To = responseBody.Contacts?.FirstOrDefault()?.Input ?? "UNKNOWN",
                WaId = responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN",
                Type = messageSentDto.Type,
                Text = messageSentDto.Text,
                Template = messageSentDto.Template,
                Direction = "outbound",
                Status = responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN",
                SentAt = DateTime.UtcNow
            };

            var meessageSignalR = MessageMapper.ToEntity(
                messageDto,
                string.IsNullOrEmpty(user) ? "System" : user,
                responseBody.Contacts?.FirstOrDefault()?.Input ?? "UNKNOWN",
                responseBody.Contacts?.FirstOrDefault()?.WaId ?? "UNKNOWN",
                responseBody.Messages?.FirstOrDefault()?.MessageStatus ?? "UNKNOWN",
                responseBody.Messages?.FirstOrDefault()?.Id
            );
            Console.WriteLine("Response content: " + jsonBody);

            // Guardar en Mongo
            await _conversationService.AddMessageAsync(newMessageSent);

            // Notificar por SignalR con MessageDto consistente
            await _hubContext.Clients.Group(meessageSignalR.To)
                .SendAsync("ReceiveMessage", meessageSignalR.To, meessageSignalR.From, meessageSignalR);

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesByWaId([FromQuery] string? WaId, [FromQuery] string? ClientId)
        {
            var messages = new List<Message>();

            if (!string.IsNullOrEmpty(ClientId) && string.IsNullOrEmpty(WaId))
            {
                messages = await _messageService.GetMessagesByClientIdAsync(ClientId);
            }
            if (!string.IsNullOrEmpty(WaId) && string.IsNullOrEmpty(ClientId))
            {
                messages = await _messageService.GetMessagesAsync(WaId);
            }

            if (messages is null)
                return NotFound();

            var messagesDto = messages.Select(m => new MessageDto()
            {
                Id = m.Id,
                ClientId = m.ClientId,
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
