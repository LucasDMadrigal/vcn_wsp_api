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
            var conversation = await _conversationRepository.GetByWaIDPhoneAsync(message.WaId);

            

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserPhone = message.To,
                    WaId = message.WaId,
                    Messages = new List<Message> { message }
                };

                await _conversationRepository.InsertAsync(conversation);
            }
            else
            {
                conversation.Messages.Add(message);
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
            var conversation = _conversationRepository.GetByWaIDPhoneAsync(waId);
            return conversation;
        }

        public Task<Conversation> GetConversationAsync(string conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Conversation>> GetConversationsAsync()
        {
            var conversations = _conversationRepository.GetConversationsAsync();
            return conversations;
        }

        public async Task UpdateMessageStatusAsync(string metaMessageId, string status)
        {
            // Traigo la conversación usando el repo
            await _messageRepository.UpdateMessageStatusAsync(metaMessageId, status);

            // Persisto el cambio
            //await _conversationRepository.UpdateAsync(conversation);
        }

    }

}
