using Chat.Domain.Entities;

namespace Chat.Services.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessagesAsync(string WaId);
        Task<List<Message>> GetMessagesByClientIdAsync(string clientId);
    }
}
