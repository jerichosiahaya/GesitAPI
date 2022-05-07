using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using GesitAPI.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using GesitAPI.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;
        public LoginController(IConfiguration config, IOptions<AppSettings> appSettings)
        {
            _config = config;
            _appSettings = appSettings.Value;
        }

        public class UserData
        {
            public string NPP { get; set; }
            public string Nama { get; set; }
            public string Jabatan { get; set; }
            public string Kelompok { get; set; }
            public string Aktif { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string expireuser { get; set; }
            public string expirepass { get; set; }
            public object userupdate { get; set; }
            public object updatedate { get; set; }
            public object admin { get; set; }
            public string Divisi { get; set; }
            public object Phone { get; set; }
            public object TimSquad { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public List<UserData> data { get; set; }
        }

        public class UserAuth
        {
            public string NPP { get; set; }
        }

        public class Response
        {
            public string Token { get; set; }
            public string NPP { get; set; }
            public string Nama { get; set; }
            public string Jabatan { get; set; }
            public string Kelompok { get; set; }
            public string Aktif { get; set; }
            public string email { get; set; }
            public string expireuser { get; set; }
            public string expirepass { get; set; }
            public object userupdate { get; set; }
            public object updatedate { get; set; }
            public object admin { get; set; }
            public string Divisi { get; set; }
            public object Phone { get; set; }
            public object TimSquad { get; set; }
        }

        [HttpGet]
        public IActionResult Get([Required]string npp, string passwordOrEmail)
        {
            var requestUrl = _config.GetValue<string>("ServerSettings:Progo:Url");
            var apiKey = _config.GetValue<string>("ServerSettings:Progo:ProgoKey");

            var client = new RestClient(requestUrl);
            client.UseNewtonsoftJson();
            var request = new RestRequest($"progodev/api/user?progo-key={apiKey}&npp={npp}");
            //request.AddHeader("progo-key", apiKey);
            //request.AddHeader("npp", npp);
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Root>(response.Content);

            if (result.status == "false")
                return BadRequest();

            var user = result.data.Where(p=>p.NPP==npp).FirstOrDefault();

            // only for generateJwtToken
            UserAuth vData = new UserAuth()
            {
                NPP = user.NPP
            };

            Response vResponse = new Response()
            {
                NPP = user.NPP,
                Nama = user.Nama,
                Token = generateJwtToken(vData),
                Divisi = user.Divisi,
                admin = user.admin,
                TimSquad = user.TimSquad,
                Aktif = user.Aktif,
                email = user.email,
                expirepass = user.expirepass,
                Jabatan = user.Jabatan,
                expireuser = user.expireuser,
                Kelompok = user.Kelompok,
                Phone = user.Phone,
                updatedate = user.updatedate,
                userupdate = user.userupdate
            };

            if (user.password == null)
            {
                if (user.email.Equals(passwordOrEmail, StringComparison.OrdinalIgnoreCase)) 
                {
                    return Ok(vResponse);
                } else
                {
                    return BadRequest();
                }
            } else
            {
                var passwordEncrypt = mD5HashEncrypt(passwordOrEmail);
                if (user.password.Equals(passwordEncrypt))
                {
                    return Ok(vResponse);
                } else
                {
                    return BadRequest();
                }
            }
        }

        private string mD5HashEncrypt(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] vByte = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < vByte.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(vByte[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        private string generateJwtToken(UserAuth user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("npp", user.NPP.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // DUMMY LOGIN
        public class UserPrivate
        {
            public int id { get; set; }
            public string npp { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string role { get; set; }
            public string password { get; set; }
        }

        private List<UserPrivate> _users = new List<UserPrivate>
        {
            new UserPrivate { id = 1, npp = "P05000", email = "amgr1@bni.com", name = "Reza Makmur", role = "AMGR", password = "dummy123" },
            new UserPrivate { id = 2, npp = "P05001", email = "avp1@bni.com", name = "Budi Septio", role = "AVP", password = "dummy123" },
            new UserPrivate { id = 3, npp = "P05002", email = "os1@bni.com", name = "Septi Anna", role = "OS", password = "dummy123" },
        };

        [HttpGet("LoginDummy")]
        public IActionResult LoginDummy([Required]string npp, string password)
        {
            var user = _users.SingleOrDefault(x => x.npp == npp && x.password == password);
            if (user == null)
            {
                return BadRequest(new { status = "Error", message = "User not found" });
            } else
            {
                UserAuth userAuth = new UserAuth()
                {
                    NPP = user.npp
                };

                UserDto userData = new UserDto()
                {
                    Id = user.id,
                    Npp = user.npp,
                    Name = user.name,
                    Role = user.role,
                    Token = generateJwtToken(userAuth)
                };
                return Ok(new { status = "Success", data = userData });
            }
        }

    }
}
