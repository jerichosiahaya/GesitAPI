using GesitAPI.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class UserData : IUser
    {
        private UserManager<IdentityUser> _userManager;

        public UserData(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<User> Authenticate(string npp, string password)
        {
            throw new NotImplementedException();
        }

        public Task Registration(User user)
        {
            throw new NotImplementedException();
        }

    }
}
