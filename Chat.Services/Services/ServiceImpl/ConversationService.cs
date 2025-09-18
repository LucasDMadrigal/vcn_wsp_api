using Chat.Domain.Entities;
using Chat.Domain.Interfaces;
using MongoDB.Driver;

namespace Chat.Services.Services.ServiceImpl
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
    public ConversationService(IConversationRepository conversationRepository, IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

        public async Task AddMessageAsync(Message message)
        {
            var conversation = await _conversationRepository.GetConversationOpenByWaIDPhoneAsync(message.WaId);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserPhone = message.To,
                    WaId = message.WaId,
                    IsOpen = true,
                    closeTimestamp = message.SentAt.AddHours(24),
                    Messages = new List<Message> { message }
                };

                await _conversationRepository.InsertAsync(conversation);
            }
            else
            {
                conversation.Messages.Add(message);
                conversation.closeTimestamp = message.SentAt.AddHours(24);
                await _conversationRepository.UpdateAsync(conversation);
            }
        }

        public  Task<Conversation> GetByMetaMessageIdAsync(string metaMessageId)
        {
            var conversation =  _conversationRepository.GetByMetaMessageIdAsync(metaMessageId);
            return conversation;
        }

        public Task<Conversation> GetByWaIDPhoneAsync(string waId)
        {
            var conversation = _conversationRepository.GetConversationOpenByWaIDPhoneAsync(waId);
            return conversation;
        }

        public Task<IEnumerable<Conversation>> GetConversationsAsync ()
        {
            var conversations = _conversationRepository.GetConversationsAsync();
            return conversations;
        }

        public Task<List<Conversation>> GetConversationsAsync(string WaId)
        {
            return _conversationRepository.GetConversationsByWaIDPhoneAsync(WaId);
        }

        public async Task UpdateMessageStatusAsync(string metaMessageId, string status)
        {
            await _messageRepository.UpdateMessageStatusAsync(metaMessageId, status);

        }

    }

}
