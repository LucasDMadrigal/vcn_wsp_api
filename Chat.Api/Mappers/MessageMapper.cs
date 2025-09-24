using Chat.Domain.Entities;
using Chat.Shared.DTOs;

namespace Chat.Api.Mappers
{
    public static class MessageMapper
    {
        public static MessageDto ToDto(Message message)
        {
            return new MessageDto
            {
                Type = message.Type,
                Text = message.Text,
                Template = message.Template,
                Direction = message.Direction
            };
        }

        public static Message ToEntity(MessageDto dto, string from, string to, string waId, string status, string metaMessageId)
        {
            return new Message
            {
                MetaMessageId = metaMessageId,
                From = from,
                To = to,
                WaId = waId,
                Type = dto.Type,
                Text = dto.Text,
                Template = dto.Template,
                Direction = dto.Direction,
                Status = status,
                SentAt = DateTime.UtcNow
            };
        }
    }
}
