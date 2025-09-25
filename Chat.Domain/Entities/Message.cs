using Chat.Shared.ApiMetaDTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Entities
{
    public class Message    
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ClientId { get; set; } = "";
        public string ConversationId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string WaId { get; set; }
        public string Type { get; set; }
        public TextMessageDto Text { get; set; }
        public TemplateMessageDto Template { get; set; }
        public string Direction { get; set; } //inbound, outbound
        public string Status { get; set; }
        public DateTime SentAt { get; set; }
        [BsonElement("metaMessageId")]
        public string? MetaMessageId { get; set; }

        [BsonElement("metaResponse")]
        public BsonDocument? MetaResponse { get; set; }
        public BsonDocument? RawPayload { get; set; }
    }
}
