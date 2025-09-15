using Chat.Domain.Entities;

namespace Chat.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessageAsync(Message message);
        Task UpdateMessageStatusAsync(string metaMessageId, string status);
        Task<Message> GetByIdAsync(string id);
        Task<Message> GetByMetaMessageIdAsync(string metaMessageId);
        Task<List<Message>> GetConversationMessagesAsync(string conversationId, int limit = 50, int skip = 0);
        Task<List<(string ConversationId, Message LastMessage)>> GetConversationsAsync(int limit = 100);
    }
}
