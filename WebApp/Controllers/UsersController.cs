#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        public IConfiguration _configuration;

        public UsersController(IUserService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> PostUser([FromBody] UserLogin user)
        {
            if (await _service.CheckIfInDB(user.userName, user.password))
            {
                User fullUser = await _service.GetByName(user.userName);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, fullUser.userName),
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: mac);
                UserToken userToken = new UserToken
                {
                    User = fullUser,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
                return Ok(userToken);
            }

            if (await _service.GetByName(user.userName) != null)
            {
                return BadRequest("Wrong password");
            }

            return BadRequest("User does not exists");
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (await _service.CheckIfInDB(user.userName, user.password))
            {
                return BadRequest("Already registerd");
            }
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                 new Claim(ClaimTypes.NameIdentifier, user.userName)
                };


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
            var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JWTParams:Issuer"],
                _configuration["JWTParams:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: mac);
            user.Contacts = new List<Contact>();
            await _service.AddToDB(user);

            UserToken userToken = new UserToken
            {
                User = user,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return Ok(userToken);
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _service.GetAll();
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _service.GetByName(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            int result = await _service.PutUser(id, user);

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


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            int result = await _service.DeleteUser(id);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}