using GesitAPI.Dtos;
using GesitAPI.Helpers;
using GesitAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        public AuthenticationController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private List<User> _users = new List<User>
        {
            new User { id = 1, npp = "P02020", email = "captainamerica@bni.com", name = "Steve Rogers", role = "GOV", password = "avengers" },
            new User { id = 2, npp = "P02021", email = "admin@bni.com", name = "Admin", role = "ADMIN", password = "Admin123" },
            new User { id = 3, npp = "P02022", email = "batman@bni.com", name = "Bruce Wayne", role = "PIC", password = "gotham" },
            new User { id = 4, npp = "P02023", email = "superman@bni.com", name = "Clark Kent", role = "MANAGEMENT", password = "metropolis" },
            new User { id = 5, npp = "P02024", email = "johndoe@bni.com", name = "John Doe", role = "PM", password = "johndoe123" }
        };

        // GET: api/<AuthenticationController>
        [HttpGet]
        public IActionResult Get(string npp, string password)
        {
            var user = _users.SingleOrDefault(x => x.npp == npp && x.password == password);
            if (user == null)
                return BadRequest(new { status = "Error", message = "User not found" });

            UserDto userData = new UserDto() { 
                Id = user.id,
                Npp = user.npp,
                Name = user.name,
                Role = user.role,
                Token = generateJwtToken(user)
            };

            return Ok(new { status = "Success", data = userData });
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
