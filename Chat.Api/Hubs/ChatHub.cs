using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string conversationId, string from, string body)
        {
            // Broadcast del mensaje a todos los que estén en la conversación
            await Clients.Group(conversationId)
                .SendAsync("ReceiveMessage", conversationId, from, body);
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
