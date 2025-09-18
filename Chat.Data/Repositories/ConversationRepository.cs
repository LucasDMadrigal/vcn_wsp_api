using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace Chat.Data.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IMongoCollection<Conversation> _conversations;

        public ConversationRepository(IMongoDatabase database)
        {
            _conversations = database.GetCollection<Conversation>("Conversations");
        }

        public async Task<Conversation?> GetByIdAsync(string id)
        {
            return await _conversations.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Conversation> GetByMetaMessageIdAsync(string metaMessageId)
        {
            return await _conversations.Find(c => c.Messages.Any(m => m.MetaMessageId == metaMessageId)).FirstOrDefaultAsync();
        }

        public async Task<Conversation?> GetConversationOpenByWaIDPhoneAsync(string waId)
        {
            var conversation = await _conversations.Find(c => c.WaId == waId && c.closeTimestamp > DateTime.UtcNow).FirstOrDefaultAsync();

            return conversation;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsAsync()
        {
            return await _conversations.Find(_ => true).ToListAsync();
        }

        public Task<List<Conversation>> GetConversationsByWaIDPhoneAsync(string waId)
        {
            return _conversations.Find(c => c.WaId == waId).ToListAsync();
        }

        public async Task<List<Conversation>> GetListConversationsByWaIDPhoneAsync(string waId)
        {
            var conversations = await _conversations.Find(c => c.WaId == waId).ToListAsync();

            return conversations;
        }

        public async Task InsertAsync(Conversation conversation)
        {
            await _conversations.InsertOneAsync(conversation);
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            await _conversations.ReplaceOneAsync(c => c.Id == conversation.Id, conversation);
        }
    }
}
