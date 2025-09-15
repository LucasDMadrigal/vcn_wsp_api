using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using Chat.Shared.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Chat.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly IMongoCollection<Conversation> _conversationCollection;

        public MessageRepository(IMongoDatabase database)
        {
            _messageCollection = database.GetCollection<Message>("messages");
            _conversationCollection = database.GetCollection<Conversation>("Conversations");
        }
        public async Task<Message> AddMessageAsync(Message message)
        {
            if (message.SentAt == default) message.SentAt = DateTime.UtcNow;
            await _messageCollection.InsertOneAsync(message); // a revisar por que deberia buscar por connversationId
            return message;
        }

        public async Task<Message> GetByIdAsync(string id)
        {
            return await _messageCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Message> GetByMetaMessageIdAsync(string metaMessageId)
        {
            return await _messageCollection.Find(m => m.MetaMessageId == metaMessageId).FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetConversationMessagesAsync(string conversationId, int limit = 50, int skip = 0)
        {
            return await _messageCollection.Find(m => m.ConversationId == conversationId)
                .SortByDescending(m => m.SentAt)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<(string ConversationId, Message LastMessage)>> GetConversationsAsync(int limit = 100)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument{{"$sort", new BsonDocument{{"Timestamp", -1}}}},
                new BsonDocument{{"$group", new BsonDocument{{"_id","$ConversationId"},{"last", new BsonDocument{{"$first","$$ROOT"}}}}}},
                new BsonDocument{{"$limit", limit}}
            };


            var result = await _messageCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return result.Select(d => (
            d.GetValue("_id").AsString,
            BsonSerializer.Deserialize<Message>(d.GetValue("last").AsBsonDocument)
            )).ToList();
        }

        public async Task UpdateMessageStatusAsync(string metaMessageId, string status)
        {
            var filter = Builders<Conversation>.Filter.ElemMatch(
                c => c.Messages,
                m => m.MetaMessageId == metaMessageId
            );

            var update = Builders<Conversation>.Update
                .Set(c => c.Messages.FirstMatchingElement().Status, status)
                .Set(c => c.Messages.FirstMatchingElement().SentAt, DateTime.UtcNow);

            var result = await _conversationCollection.UpdateOneAsync(filter, update);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");

            if (result.MatchedCount == 0)
            {
                Console.WriteLine($"⚠️ No se encontró ningún mensaje con MetaMessageId={metaMessageId}");
                return;
            }

            // 🔎 Obtener el documento actuali


        }
    }
}
