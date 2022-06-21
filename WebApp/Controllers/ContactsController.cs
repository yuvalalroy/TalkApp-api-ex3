#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;
using WebApp.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApp.Hubs;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class contactsController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMessageService _messagesService;
        public IConfiguration _configuration;
        public INotificationService _notificationService;

        public contactsController(IContactService service, IUserService userService, IMessageService messagesService, IConfiguration configuration, INotificationService notificationService)
        {
            _contactService = service;
            _userService = userService;
            _messagesService = messagesService;
            _configuration = configuration;
            _notificationService = notificationService;
        }


        private async Task<User> getUser()
        {
            var currentUser = HttpContext.User;
            string userName = null;
            User user = null;
            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userName = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            if (userName != null)
            {
                user = await _userService.GetByName(userName);
            }
            return user;
                
        }

        // GET: api/Contacts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            User user = await getUser();
            if (user != null)
            {
                List<Contact> l = _contactService.GetContactsByUserName(user.userName).ToList();
             return Ok(_contactService.GetContactsByUserName(user.userName).ToList());
            }
            return BadRequest("didn't find user");
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Contact>> GetContact(string id)
        {
            User user = await getUser();
            var contact = await _contactService.GetContact(user.userName, id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutContact(string id, Contact contact)
        {
            contact.id = id;
            User user = await getUser();
            Contact c = await _contactService.GetContact(user.userName, id);
            c.name = contact.name;
            int result = await _contactService.PutContact(id, c);

            if (result == -1)
            {
                return BadRequest();
            }
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();

        }

        // POST: api/contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact([Bind("id, User, name, lastdate, server, last, Messages")] Contact contact)
        {

            User user = await getUser();

            if (await _contactService.CheckIfInUserContacts(user.userName, contact.id))
            {
                return BadRequest("Contact already exists");
            }

            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan time = new TimeSpan(now.Hours, now.Minutes, 0);
            contact.User = user;
            contact.Messages = new List<Message>();
            contact.last = null;
            contact.lastdate = null;

            await _contactService.AddToDB(contact);

            return CreatedAtAction("PostContact", new { id = contact.Identifier }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContact(string id)
        {
            User user = await getUser();
            Contact c = await _contactService.GetContact(user.userName, id);
            int result = await _contactService.DeleteContact(c.Identifier);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }


        // GET: api/Contact/5/Messages
        [HttpGet("{id}/Messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string id)
        {
            User user = await getUser();
            Contact contact = await _contactService.GetContact(user.userName, id);
            return Ok(_contactService.GetMessagesByContact(contact));
        }


        // GET: api/Contact/5/Messages/181
        [HttpGet("{id}/Messages/{id2}")]
        public async Task<ActionResult<Message>> GetMessage(int id2)
        {
            var message = await _messagesService.GetMessage(id2);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Contact/5/Messages/123
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/Messages/{id2}")]
        public async Task<IActionResult> PutMessage(int id2, Message newMessage)
        {
            Message message = await _messagesService.GetMessage(id2);
            message.content = newMessage.content;
            int result = await _messagesService.PutMessage(id2, message);

            if (result == -1)
            {
                return BadRequest();
            }
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();
        }


        // POST: api/Contacts/5/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/Messages")]
        public async Task<ActionResult<Message>> PostMessage(string id, [Bind("id, content, created, sent, Contact")] Message message)
        {
            User user = await getUser();
            message.Contact = await _contactService.GetContact(user.userName, id);
            message.created = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            message.sent = true;

            await _messagesService.AddToDB(message);
            await _contactService.UpdateLastDate(message.created, message.Contact);
            await _contactService.UpdateLastMessage(message.content, message.Contact);

            return CreatedAtAction("PostMessage", new { id = message.id }, message);
        }

        // DELETE: api/Contacts/5/Messages/5
        [HttpDelete("{id}/Messages/{id2}")]
        public async Task<IActionResult> DeleteMessage(int id2)
        {
            int result = await _messagesService.DeleteMessage(id2);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize]
        [HttpPost("registerDevice")]
        public async Task<IActionResult> registerDevice(AndroidDeviceIDModel androidDeviceIDModel)
        {
            var user = await getUser();
            if (user == null)
            {
                return NotFound("Invalid details");
            }

            var androidDeviceID = new AndroidDeviceIDModel
            {
                DeviceId = androidDeviceIDModel.DeviceId
            };

            await _notificationService.CreateAndoridDeviceOfUser(androidDeviceID, user.userName);
            return Ok();
        }

    }
}
