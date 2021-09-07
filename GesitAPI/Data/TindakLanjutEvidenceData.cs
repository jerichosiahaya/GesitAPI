using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class TindakLanjutEvidenceData : ITindakLanjutEvidence
    {
        private readonly GesitDbContext _db;
        public TindakLanjutEvidenceData(GesitDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException(); 
        }

        public async Task<IEnumerable<TindakLanjutEvidence>> CountExistingFileNameTLEvidence(string filename)
        {
            var result = await _db.TindakLanjutEvidences.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<TindakLanjutEvidence>> GetAll()
        {
            var result = await _db.TindakLanjutEvidences.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<TindakLanjutEvidence> GetById(string id)
        {
            var result = await _db.TindakLanjutEvidences.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<TindakLanjutEvidence>> GetByTindakLanjutID(string idTL)
        {
            var result = await _db.TindakLanjutEvidences.Where(s => s.TindaklanjutId == Convert.ToInt32(idTL)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task Insert(TindakLanjutEvidence obj)
        {
            try
            {
                _db.TindakLanjutEvidences.Add(obj);
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

        public Task Update(string id, TindakLanjutEvidence obj)
        {
            throw new NotImplementedException();
        }
    }
}
