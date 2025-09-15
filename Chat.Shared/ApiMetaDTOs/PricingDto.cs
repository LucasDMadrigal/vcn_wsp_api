using System.Text.Json.Serialization;

namespace Chat.Shared.ApiMetaDTOs
{
    public class PricingDto
    {
        [JsonPropertyName("billable")]
        public bool Billable { get; set; }

        [JsonPropertyName("pricing_model")]
        public string PricingModel { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}