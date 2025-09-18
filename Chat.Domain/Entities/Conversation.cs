using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Entities
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserPhone { get; set; } = string.Empty;
        public string WaId { get; set; } = string.Empty;
        public DateTime? closeTimestamp { get; set; }
        public bool IsOpen { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
}
}
