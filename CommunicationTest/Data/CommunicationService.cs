using Data;

namespace CommunicationTest.Data
{
    public class CommunicationService
    {
        private readonly IMessageRepository _messageRepository;

        public CommunicationService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
        }

        public async Task<List<Message>> GetCommunicationData()
        {
            return await _messageRepository.GetAll();
        }

        public void SendMessageToQueue(Message message)
        {
            //send message to rabbitMQ
            QueueProducer.Publish(message.MessageString);
            Console.WriteLine("I produced the message - " + message.MessageString);
        }
    }
}