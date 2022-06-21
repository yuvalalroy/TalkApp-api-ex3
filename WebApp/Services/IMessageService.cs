using WebApp.Models;


namespace WebApp.Services
{

    public interface IMessageService
    {
        public Task<List<Message>> GetMessages();
        public Task<Message> GetMessage(int id);

        public bool MessageExists(int id);

        public Task<int> PutMessage(int id, Message message);

        public Task<bool> AddToDB(Message message);

        public Task<int> DeleteMessage(int id);

    }
}
