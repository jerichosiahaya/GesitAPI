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

        public async Task<IEnumerable<Rha>> CountExistingFileNameRha(string filename)
        {
            var result = await _db.Rhas.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task Delete(string id)
        {
            var result = await GetById(id);
            if (result != null)
            {
                try
                {
                    _db.Rhas.Remove(result);
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

        public async Task DeleteAll(string id)
        {
            var result = await _db.Rhas.Where(s => s.Id == Convert.ToInt32(id))
                .FirstOrDefaultAsync();

            if (result != null)
            {
                try
                {
                    _db.Rhas.Remove(result);
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

        public async Task<IEnumerable<Rha>> GetAll()
        {
            var result = await _db.Rhas.Include(e => e.SubRhas).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            // ThenInclude(c => c.InputTlfilesEvidences). kalau mau tambah tindak lanjut evidences
            return result;
        }
        public async Task<Rha> GetById(string id)
        {
            var result = await _db.Rhas.Where(s => s.Id == Convert.ToInt32(id)).Include(c => c.SubRhas).ThenInclude(b=>b.SubRhaimages).FirstOrDefaultAsync();
            return result;
        }

        // GET Rha only with Sub RHA assign
        public  IEnumerable<Rha> GetSubRHAByAssign(string assign)
        {
            //SubRha sbrha = new SubRha();
            //var result = await _db.Rhas.Include(c => c.SubRhas.Where(o => o.Assign == assign))
            //                           //.ThenInclude(o => o.SubRhaevidences)
            //                           .Where(x => x.SubRhas.Any(c=>c.Assign != null))
            //                           .AsNoTracking()
            //                           .ToListAsync();

            var result = _db.Rhas.SelectMany(r => r.SubRhas, (r, s) => new { r = r, s = s })
                             .Where(temp0 => (temp0.s.Assign == assign) && temp0.s.OpenClose == "Open")
                             .Select(temp0 => temp0.r)
                             .Distinct();

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

        // TBC
        public async Task Update(string id, Rha obj)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.Kondisi = obj.Kondisi;
                    result.SubKondisi = obj.SubKondisi;
                    result.Rekomendasi = obj.Rekomendasi;
                    result.TargetDate = obj.TargetDate;
                    result.Assign = obj.Assign;
                    result.CreatedBy = obj.CreatedBy;
                    result.DirSekor = obj.DirSekor;
                    result.StatusJt = obj.StatusJt;
                    result.StatusTemuan = obj.StatusTemuan;
                    result.Uic = obj.Uic;
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
