using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class StatusDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("recipient_id")]
        public string RecipientId { get; set; }

        [JsonPropertyName("conversation")]
        public ConversationDto Conversation { get; set; }

        [JsonPropertyName("pricing")]
        public PricingDto Pricing { get; set; }
    }
}