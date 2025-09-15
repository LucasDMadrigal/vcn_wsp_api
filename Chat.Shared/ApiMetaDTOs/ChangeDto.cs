using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class ChangeDto
    {
        [JsonPropertyName("value")]
        public ValueDto Value { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }
    }
}