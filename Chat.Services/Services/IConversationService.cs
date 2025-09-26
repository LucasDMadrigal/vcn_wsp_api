using Chat.Domain.Entities;
using Chat.Shared.DTOs;

namespace Chat.Services.Services
{
    public interface IConversationService
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Conversation>> GetConversationsAsync();
        Task<List<string>> GetDistinctWaIdConversationsAsync();
        Task<List<Conversation>> GetConversationsAsync(string WaId);
        Task UpdateMessageStatusAsync(string metaMessageId, string status);
        Task<Conversation> GetByMetaMessageIdAsync(string metaMessageId);
        Task<Conversation> GetByWaIDPhoneAsync(string waId);
        Task<Conversation> GetByClientIdAsync(string clientId);
    }
}
