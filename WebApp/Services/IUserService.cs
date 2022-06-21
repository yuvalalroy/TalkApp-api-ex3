using WebApp.Models;
namespace WebApp.Services
{
    public interface IUserService
    {
        public Task<bool> AddToDB(User user);

        public Task<bool> CheckIfInDB(string name, string password);

        public Task<User?> GetByName(string name);

       // public List<Contact> GetAllContacts(string id);

        public Task<List<User>> GetAll();

        public Task<int> PutUser(string name, User user);

        public Task<int> DeleteUser(string name);

        public bool UserExists(string name);




    }
}
