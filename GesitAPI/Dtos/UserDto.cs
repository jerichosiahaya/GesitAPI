using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Npp { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.Now;
    }
}
