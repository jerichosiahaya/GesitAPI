using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class RhaevidenceData : IRhaevidence
    {
        private GesitDbContext _db;
        public RhaevidenceData(GesitDbContext db)
        {
            _db = db;   
        }

        public async Task<IEnumerable<Rhaevidence>> CountExistingFileNameRhaEvidence(string filename)
        {
            var result = await _db.Rhaevidences.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        // TBC
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Rhaevidence>> GetAll()
        {
            var result = await _db.Rhaevidences.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Rhaevidence> GetById(string id)
        {
            var result = await _db.Rhaevidences.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task Insert(Rhaevidence obj)
        {
            try
            {
                _db.Rhaevidences.Add(obj);
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

        // TBC
        public Task Update(string id, Rhaevidence obj)
        {
            throw new NotImplementedException();
        }
    }
}
