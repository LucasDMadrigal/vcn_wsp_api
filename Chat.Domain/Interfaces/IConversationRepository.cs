using Chat.Domain.Entities;

namespace Chat.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> GetConversationOpenByWaIDPhoneAsync(string waId);
        Task<List<Conversation>> GetListConversationsByClientIdPhoneAsync(string clientId);
        Task<List<Conversation>> GetListConversationsByWaIDPhoneAsync(string waId);
        Task<Conversation> GetByIdAsync(string id);
        Task<Conversation> GetByMetaMessageIdAsync(string metaMessageId);
        Task<Conversation> GetByClientIdAsync(string clientId);
        Task InsertAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        Task<IEnumerable<Conversation>> GetConversationsAsync();
        Task <List<Conversation>> GetConversationsByWaIDPhoneAsync(string waId);
    }
}
