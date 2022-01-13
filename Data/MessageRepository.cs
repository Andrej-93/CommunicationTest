using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Message entity)
        {
            _context.Messages.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetAll()
        {
            return await _context.Messages.ToListAsync();
        }
    }
}