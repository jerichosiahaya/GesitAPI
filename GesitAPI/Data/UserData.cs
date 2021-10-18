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

        public async Task Registration(User user)
        {
            try
            {
                var _user = new IdentityUser { UserName = user.npp, Email = user.name };
                var result = await _userManager.CreateAsync(_user, user.password);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
