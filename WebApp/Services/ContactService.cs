using WebApp.Data;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Services
{
    public class ContactService : IContactService
    {

        private readonly WebAppContext _context;

        public ContactService(WebAppContext context)
        {
            _context = context;
        }


        public async Task<Contact> GetContact(string userName, string id)
        {
            return await _context.Contact.FirstOrDefaultAsync(c => c.id.Equals(id) && c.User.userName.Equals(userName));
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            return await _context.Contact.ToListAsync();
        }

        public async Task<bool> CheckIfInDB(string name)
        {
            return await _context.Contact.AnyAsync(c => c.id == name);
        }

        public async Task<bool> CheckIfInUserContacts(string userName, string contactName)
        {
            return await _context.Contact.AnyAsync(c => c.id.Equals(contactName) && c.User.userName.Equals(userName));
        }

        public async Task<bool> AddToDB(Contact contact)
        {
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> PutContact(string id, Contact contact)
        {
            if (id != contact.id)
            {
                return -1;
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        public async Task<int> UpdateLastDate(string time, Contact contact)
        {
            contact.lastdate = time;

            _context.Entry(contact).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return 1;

        }

        public async Task<int> DeleteContact(int identifier)
        {
            var contact = await _context.Contact.FindAsync(identifier);
            if (contact == null)
            {
                return -1;
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return 1;
        }

        public bool ContactExists(string name)
        {
            return _context.Contact.Any(c => c.id == name);
        }


        public IQueryable<Contact> GetContactsByUserName(string name)
        {
            var contacts = from contact in _context.Contact
                           where contact.User.userName == name

                           select contact;

            return contacts;
        }

        public IQueryable<Message> GetMessagesByContact(Contact contact)
        {
            var messages = from message in _context.Message
                           where message.Contact.id == contact.id
                           select message;

            return messages;
        }

        public async Task<int> UpdateLastMessage(string content, Contact contact)
        {
            contact.last = content;

            _context.Entry(contact).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return 1;

        }
    }
}
