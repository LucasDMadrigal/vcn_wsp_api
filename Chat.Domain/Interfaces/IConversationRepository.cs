using Chat.Domain.Entities;

namespace Chat.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> GetByWaIDPhoneAsync(string userPhone);
        Task<Conversation> GetByIdAsync(string id);
        Task<Conversation> GetByMetaMessageIdAsync(string metaMessageId);
        Task InsertAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        Task<IEnumerable<Conversation>> GetConversationsAsync();
    }
}
