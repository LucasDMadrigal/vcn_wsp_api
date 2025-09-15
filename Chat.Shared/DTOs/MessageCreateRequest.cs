namespace Chat.Shared.DTOs
{
    public record MessageCreateRequest
    {
        public string ConversationId { get; set; } = String.Empty;
        public string Text { get; set; } = String.Empty;
        public string to { get; set; } = String.Empty;
    }
}
