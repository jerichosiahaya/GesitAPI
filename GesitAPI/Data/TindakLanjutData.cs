using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class TindakLanjutData : ITindakLanjut
    {
        private GesitDbContext _db;
        public TindakLanjutData(GesitDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<TindakLanjut>> CountExistingFileNameTindakLanjut(string filename)
        {
            var result = await _db.TindakLanjuts.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        // TBC
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TindakLanjut>> GetAll()
        {
            var result = await _db.TindakLanjuts.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<TindakLanjut> GetById(string id)
        {
            var result = await _db.TindakLanjuts.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public Task<IEnumerable<TindakLanjut>> GetByRhaID(string idRha)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TindakLanjut>> GetBySubRhaID(string idRha)
        {
            var result = await _db.TindakLanjuts.OrderByDescending(s => s.CreatedAt).Where(s => s.SubRhaId == Convert.ToInt32(idRha)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task Insert(TindakLanjut obj)
        {
            try
            {
                _db.TindakLanjuts.Add(obj);
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
        public Task Update(string id, TindakLanjut obj)
        {
            throw new NotImplementedException();
        }
    }
}
