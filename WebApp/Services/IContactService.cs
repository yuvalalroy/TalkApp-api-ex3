using WebApp.Models;
namespace WebApp.Services
{
    public interface IContactService
    {

        public Task<Contact> GetContact(string userName, string id);

        public Task<List<Contact>> GetAllContacts();

        public Task<bool> CheckIfInDB(string id);

        public Task<bool> AddToDB(Contact contact);

        public Task<int> PutContact(string id, Contact contact);

        public Task<int> UpdateLastDate(string time, Contact contact);

        public Task<int> UpdateLastMessage(string content, Contact contact);

        public Task<int> DeleteContact(int identifier);

        public IQueryable<Contact> GetContactsByUserName(string name);

        public IQueryable<Message> GetMessagesByContact(Contact contact);

        public Task<bool> CheckIfInUserContacts(string userName, string contactId);

    }
}
