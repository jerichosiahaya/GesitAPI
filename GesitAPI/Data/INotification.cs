using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface INotification : ICrud<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationByProjectId(string projectId);
        Task<IEnumerable<Notification>> GetActive();
        Task<IEnumerable<Notification>> GetNotActive();
    }
}
