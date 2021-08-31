using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GesitAPI.Models
{
    public class User
    {
        public int id { get; set; }
        public string npp { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        [JsonIgnore]
        public string password { get; set; }
    }
}
