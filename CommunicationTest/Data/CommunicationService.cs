using Data;

namespace CommunicationTest.Data
{
    public class CommunicationService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConfiguration _configuration;

        public CommunicationService(IMessageRepository messageRepository, IConfiguration configuration)
        {
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<Message>> GetCommunicationData()
        {
            return await _messageRepository.GetAll();
        }

        public void SendMessageToQueue(Message message)
        {
            var uriSetting = _configuration.GetValue<string>("EventBusSettings:HostAddress");
            //send message to rabbitMQ
            QueueProducer.Publish(message.MessageString, uriSetting);
            Console.WriteLine("I produced the message - " + message.MessageString);
        }
    }
}