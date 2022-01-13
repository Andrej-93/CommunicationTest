namespace Data
{
    public interface IMessageRepository
    {
        Task Add(Message entity);
        Task<List<Message>> GetAll();
    }
}