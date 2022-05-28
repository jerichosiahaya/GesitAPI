using GesitAPI.Dtos;
using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class SubRhaEvidenceData : ISubRhaEvidence
    {
        GesitDbContext _db;
        public SubRhaEvidenceData(GesitDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<SubRhaevidence>> CountExistingFileNameSubRhaEvidence(string filename)
        {
            var result = await _db.SubRhaevidences.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        // TBC
        public async Task Delete(string id)
        {
            var result = await GetById(id);

            if (result != null)
            {
                try
                {
                    _db.SubRhaevidences.Remove(result);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {

                    throw new Exception($"DbError: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error: {ex.Message}");
                }
            }
        }

        public async Task<IEnumerable<SubRhaevidence>> GetAll()
        {
            var result = await _db.SubRhaevidences.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<SubRhaevidence> GetById(string id)
        {
            var result = await _db.SubRhaevidences.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }
        public async Task Insert(SubRhaevidence obj)
        {
            try
            {
                _db.SubRhaevidences.Add(obj);
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
        public Task Update(string id, SubRhaevidence obj)
        {
            throw new NotImplementedException();
        }
    }
}
