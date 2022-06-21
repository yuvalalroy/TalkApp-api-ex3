using WebApp.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly WebAppContext _context;

        public UserService(WebAppContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToDB(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public  async Task<bool> CheckIfInDB(string name, string password)
        {
            return await _context.User.AnyAsync(e => e.userName.Equals(name) && e.password.Equals(password));
        }


        public async Task<List<User>> GetAll()
        {
            return await _context.User.ToListAsync();
        }


        public async Task<User?> GetByName(string name)
        {
            var users = await GetAll();
            var user = users.Find(m => m.userName.Equals(name));

            return user;
        }


        public async Task<int> PutUser(string name, User user)
        {
            if (name != user.userName)
            {
                return -1;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(name))
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

        public async Task<int> DeleteUser(string id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return -1;
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return 1;
        }

        public bool UserExists(string name)
        {
            return _context.User.Any(e => e.userName == name);

        }


    }
}
