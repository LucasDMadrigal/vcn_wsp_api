using Chat.Api.Hubs;
using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using Chat.Services.Services;
using Chat.Shared.ApiMetaDTOs;
using Chat.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly string _verifyToken;
        private readonly string _appSecret;
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationService _conversationService;

        private readonly IHubContext<ChatHub> _hub;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IConfiguration config,
            IConversationService conversationService,
            IMessageRepository messageRepository, 
            IHubContext<ChatHub> hub, 
            ILogger<WebhookController> logger
            )
        {
            _verifyToken = config["WebhookVerificationToken"] ?? "mi_token_por_defecto";
            _appSecret = config["Meta:AppSecret"] ?? "";
            _messageRepository = messageRepository;
            _hub = hub;
            _logger = logger;
            _conversationService = conversationService;
        }

        // GET verification
        [HttpGet]
        public IActionResult Verify([FromQuery(Name = "hub.mode")] string mode,
                                    [FromQuery(Name = "hub.challenge")] string challenge,
                                    [FromQuery(Name = "hub.verify_token")] string token)
        {
            _logger.LogInformation("GET /webhook recibido. mode={Mode}, token={Token}, challenge={Challenge}", mode, token, challenge);

            if (mode == "subscribe" && token == _verifyToken)
            {
                _logger.LogInformation("✅ WEBHOOK VERIFIED");
                return Content(challenge);
            }

            return Forbid();
        }
        // POST inbound events
        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] JsonElement body)
        {
            // Validate signature if header present
            if (!string.IsNullOrEmpty(_appSecret) && Request.Headers.TryGetValue("X-Hub-Signature-256", out var sigHeader))
            {
                var expectedSig = "sha256=" + ComputeHmacSha256(Request.Body, _appSecret);
                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(expectedSig),
                        Encoding.UTF8.GetBytes(sigHeader)))
                {
                    _logger.LogWarning("❌ Invalid signature");
                    return Forbid();
                }
            }

            try
            {
                var metaResponseObj = body.GetProperty("object");
                var entry = body.GetProperty("entry")[0];
                var changes = entry.GetProperty("changes")[0];
                var value = changes.GetProperty("value");

                if (value.TryGetProperty("messages", out var messages))
                {
                    foreach (var m in messages.EnumerateArray())
                    {
                        var from = m.GetProperty("from").GetString();
                        var msgId = m.GetProperty("id").GetString();
                        var timestamp = m.GetProperty("timestamp").GetString();
                        var type = m.GetProperty("type").GetString();

                        // body del texto si es type=text
                        var textBody = m.TryGetProperty("text", out var t)
                            ? t.GetProperty("body").GetString()
                            : null;

                        
                        var message = new Message
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            ConversationId = string.Empty,
                            From = from ?? "",
                            To = value.GetProperty("metadata").GetProperty("phone_number_id").GetString() ?? "",
                            WaId = from ?? "",
                            Type = type,
                            Text = new TextMessageDto { Body = textBody ?? "" },
                            Template = null,
                            Direction = "inbound",
                            Status = "received",
                            SentAt = DateTimeOffset
                                        .FromUnixTimeSeconds(long.Parse(timestamp))
                                        .UtcDateTime,
                            MetaMessageId = msgId,
                            MetaResponse = null,                        
                            RawPayload = BsonDocument.Parse(m.ToString()) 
                        };

                        
                        await _conversationService.AddMessageAsync(message);
                        
                        var dto = new MessageDto
                        {
                            Id = message.Id,
                            From = message.From,
                            To = message.To,
                            Text = message.Text,
                            Status = message.Status
                        };

                        await _hub.Clients.Group(from!).SendAsync("ReceiveMessage", dto);
                    }
                }

                if (value.TryGetProperty("statuses", out var statuses))
                {
                    foreach (var s in statuses.EnumerateArray())
                    {
                        var metaMsgId = s.GetProperty("id").GetString();
                        var status = s.GetProperty("status").ToString();
                        var conversationId = s.GetProperty("recipient_id").GetString();
                        var statusString = s.GetProperty("status").GetString();


                        var dto = new MessageStatusDto
                        {
                            MetaMessageId = metaMsgId ?? "",
                            ConversationId = conversationId ?? "",
                            Status = statusString,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _hub.Clients.Group(dto.ConversationId).SendAsync("MessageStatusUpdated", dto);

                        await _conversationService.UpdateMessageStatusAsync(metaMsgId, status);
                    }
                }
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing webhook body");
            }
            return Ok();
        }

        private static string ComputeHmacSha256(Stream bodyStream, string appSecret)
        {
            bodyStream.Position = 0;
            using var reader = new StreamReader(bodyStream, Encoding.UTF8, leaveOpen: true);
            var body = reader.ReadToEnd();
            bodyStream.Position = 0;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
