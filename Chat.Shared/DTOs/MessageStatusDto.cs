namespace Chat.Shared.DTOs
{
    public record MessageStatusDto 
    {
        public string MetaMessageId { get; set; } = default!;
        public string ConversationId { get; set; } = default!;
        public string Status { get; set; } = default!;   // sent, delivered, read
        public DateTime UpdatedAt { get; set; }
    }
}
