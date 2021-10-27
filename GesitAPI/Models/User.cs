using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class User
    {
        public string Npp { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Division { get; set; }
        public string Role { get; set; }
        public int? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
