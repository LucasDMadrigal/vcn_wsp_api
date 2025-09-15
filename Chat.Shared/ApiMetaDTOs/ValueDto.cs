using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class ValueDto
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public MetadataDto Metadata { get; set; }

        [JsonPropertyName("statuses")]
        public List<StatusDto> Statuses { get; set; }
    }
}