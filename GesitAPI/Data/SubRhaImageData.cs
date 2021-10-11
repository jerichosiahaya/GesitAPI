using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class SubRhaImageData : ISubRhaImage
    {
        GesitDbContext _db;
        public SubRhaImageData(GesitDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SubRhaimage>> CountExistingFileNameSubRhaImage(string filename)
        {
            var result = await _db.SubRhaimages.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SubRhaimage>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<SubRhaimage> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(SubRhaimage obj)
        {
            try
            {
                _db.SubRhaimages.Add(obj);
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

        public Task Update(string id, SubRhaimage obj)
        {
            throw new NotImplementedException();
        }
    }
}
