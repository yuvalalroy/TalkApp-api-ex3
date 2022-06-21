using WebApp.Data;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;
namespace WebApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly WebAppContext _context;

        public MessageService(WebAppContext context)
        {
            _context = context;
        }

        public bool MessageExists(int id)
        {
            return _context.Message.Any(c => c.id == id);
        }

        public async Task<List<Message>> GetMessages()
        {
            return await _context.Message.ToListAsync();
        }


        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message.FindAsync(id);
        }


        public async Task<bool> AddToDB(Message message)
        {
            _context.Message.Add(message);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> PutMessage(int id, Message message)
        {
            if (id != message.id)
            {
                return -1;
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return 0;
                }
                else
                {
                    throw;
                }
            }
            return 1;
        }


        public async Task<int> DeleteMessage(int id)
        {
            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return -1;
            }

            _context.Message.Remove(message);
            await _context.SaveChangesAsync();

            return 1;
        }
    }
}
