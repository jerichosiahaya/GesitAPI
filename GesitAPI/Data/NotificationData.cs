using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class NotificationData : INotification
    {
        private GesitDbContext _db;
        public NotificationData(GesitDbContext db)
        {
            _db = db;
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Notification>> GetActive()
        {
            var result = await _db.Notifications.Where(p=>p.Status == 0).OrderBy(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Notification>> GetAll()
        {
            var result = await _db.Notifications.OrderBy(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Notification> GetById(string id)
        {
            var result = await _db.Notifications.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<Notification>> GetNotActive()
        {
            var result = await _db.Notifications.Where(p => p.Status == 1).OrderBy(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Notification>> GetNotificationByProjectId(string projectId)
        {
            var result = await _db.Notifications.Where(o => o.ProjectId == projectId).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task Insert(Notification obj)
        {
            try
            {
                _db.Notifications.Add(obj);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Update(string id, Notification obj)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.ProjectId = obj.ProjectId;
                    result.ProjectCategory = obj.ProjectCategory;
                    result.ProjectTitle = obj.ProjectTitle;
                    result.ProjectDocument = obj.ProjectDocument;
                    result.TargetDate = obj.TargetDate;
                    result.AssignedBy = obj.AssignedBy;
                    result.AssignedFor = obj.AssignedFor;
                    result.Status = obj.Status;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Data {id} not found");
                }
            }
            catch (DbUpdateException DbEx)
            {

                throw new Exception(DbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
