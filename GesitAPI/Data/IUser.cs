using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface IUser
    {
        Task<User> Authenticate(string npp, string password);
        Task Registration(User user);
    }
}
