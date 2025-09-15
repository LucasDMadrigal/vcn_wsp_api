namespace Chat.Shared.DTOs
{
    public record MessageDto
    {
        public string Id { get; init; }
        public string ConversationId { get; init; }
        public string From { get; init; }
        public string To { get; init; }
        public string Content { get; init; }
        public MessageDirection Direction { get; init; }
        public string Status { get; init; }
        public DateTimeOffset SentAt { get; init; }
        public string MetaMessageId { get; init; }
    }
}
