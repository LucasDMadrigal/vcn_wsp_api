using Chat.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Chat.Api.Hubs
{
    public class ChatHub : Hub
    {
        // Enviar mensaje desde cliente → a todos en el grupo (la conversación)
        public async Task SendMessage(string conversationId, string from, MessageDto message)
        {
            // Difunde el mismo objeto MessageDto a todos en el grupo
            await Clients.Group(conversationId)
                .SendAsync("ReceiveMessage", conversationId, from, message);
        }


        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var conversationId = httpContext?.Request.Query["conversationId"];

            if (!string.IsNullOrEmpty(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }

            await base.OnConnectedAsync();
        }
    }
}
