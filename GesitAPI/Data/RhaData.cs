using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class RhaData : IRha
    {
        private GesitDbContext _db;
        public RhaData(GesitDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Rha>> GetAll()
        {
            var result = await _db.Rhas.Include(e => e.Rhaevidences).Include(e => e.TindakLanjuts).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            // ThenInclude(c => c.InputTlfilesEvidences). kalau mau tambah tindak lanjut evidences
            return result;
        }

        public async Task<Rha> GetById(string id)
        {
            var result = await _db.Rhas.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task Insert(Rha obj)
        {
            try
            {
                _db.Rhas.Add(obj);
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

        public Task Update(string id, Rha obj)
        {
            throw new NotImplementedException();
        }
    }
}
