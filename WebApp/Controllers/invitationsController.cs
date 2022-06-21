using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.Hubs;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class invitationsController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private ContactHub _contactHub;


        public invitationsController(IContactService contactService, IUserService userService, ContactHub contactHub)
        {
            _contactService = contactService;
            _userService = userService;
            _contactHub = contactHub;
        }


        // POST: api/Invitations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Invitation>> PostInvitation([Bind("from, to, server")] Invitation invitation)
        {
            //User userToAdd = await _userService.GetByName(invitation.from);
            User currentUser = await _userService.GetByName(invitation.to);

            if (currentUser == null)
            {
                return BadRequest("User does not exist");
            }

            if (await _contactService.CheckIfInUserContacts(invitation.to, invitation.from))
            {
                return BadRequest("Contact already exists");
            }

            Contact contact = new Contact();
            contact.id = invitation.from;
            contact.name = invitation.from;
            contact.User = currentUser;
            contact.Messages = new List<Message>();
            contact.server = invitation.server;
            contact.last = null;
            contact.lastdate = null;

            await _contactService.AddToDB(contact);
            await _contactHub.AddContact(invitation.to, contact);

            return CreatedAtAction("PostInvitation", new { id = contact.Identifier }, contact);

        }
    }


}
