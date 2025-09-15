using Chat.Services.Services;
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
        public IActionResult GetConversationsByUserPhone([FromRoute] string waId)
       {
           var conversation = _conversationService.GetByWaIDPhoneAsync(waId);

            if (conversation == null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
       }
    }
}
