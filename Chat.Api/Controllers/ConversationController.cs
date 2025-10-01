using Chat.Services.Services;
using Chat.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ConversationController : ControllerBase
        {
            IConversationService _conversationService;

            public ConversationController(IConversationService conversationService)
            {
                _conversationService = conversationService;
            }

            [HttpGet("{waId}")]
            public async Task<IActionResult> GetOpenConversationByWaId([FromRoute] string waId)
            {
                var conversation = await _conversationService.GetByWaIDPhoneAsync(waId);
                var convDto = new ConversationResponseDto();
                convDto.Id = conversation.Id;
                convDto.WaId = conversation.WaId;
                convDto.Messages = conversation.Messages.Select(m => new MessageDto()
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
                }).ToList();

                return Ok(convDto);
            }
            
            [HttpGet("{clientId}")]
            public async Task<IActionResult> GetConversationByClientId([FromRoute] string clientId)
            {
                var conversation = await _conversationService.GetByClientIdAsync(clientId);
                var convDto = new ConversationResponseDto();
                convDto.Id = conversation.Id;
                convDto.WaId = conversation.WaId;
                convDto.Messages = conversation.Messages.Select(m => new MessageDto()
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
                }).ToList();

                return Ok(convDto);
            }

            [HttpGet("All/{waId}")]
            public async Task<IActionResult> GetAllConversationsByWaId([FromRoute] string waId)
            {
                var conversations = await _conversationService.GetConversationsAsync(waId);
                var convDto = conversations.Select(c => new ConversationResponseDto()
                {
                    Id = c.Id,
                    WaId = c.WaId,
                    ClientId = c.ClientId,
                    IsOpen = c.IsOpen,
                    Messages = c.Messages.Select(m => new MessageDto()
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
                    }).ToList()
                }).ToList();
                return Ok(convDto);
            }

            [HttpGet("AllWaId")]
            public async Task<IActionResult> GetAllUnicWaIdConversations()
            {
                var conversationWaIds = await _conversationService.GetDistinctWaIdConversationsAsync();

                return Ok(conversationWaIds);
            }
        }
}
