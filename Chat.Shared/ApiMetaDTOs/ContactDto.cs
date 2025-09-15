using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class ContactDto
    {
        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("wa_id")]
        public string WaId { get; set; }
    }
}