using AutoMapper;
using GesitAPI.Dtos;
using GesitAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class SubRhaData : ISubRha
    {
        private GesitDbContext _db;
        public SubRhaData(GesitDbContext db)
        {
            _db = db;
        }

        // NOT NECESSARY
        public Task<IEnumerable<SubRha>> CountExistingFileNameSubRha(string filename)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            var result = await GetById(id);
            if (result != null)
            {
                try
                {
                    _db.SubRhas.Remove(result);
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

        public async Task<IEnumerable<SubRha>> GetAll()
        {
            var result = await _db.SubRhas.Include(e => e.SubRhaevidences).Include(c => c.TindakLanjuts).Include(p=>p.SubRhaimages).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetAllTracking()
        {
            var result = await _db.SubRhas.OrderByDescending(s => s.CreatedAt).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByAssign(string assign)
        {
            var result = await _db.SubRhas.Where(s => s.Assign == assign).Include(e => e.TindakLanjuts).ThenInclude(o => o.TindakLanjutEvidences).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<SubRha> GetById(string id)
        {
            var result = await _db.SubRhas.Where(s => s.Id == Convert.ToInt32(id)).Include(s=>s.SubRhaevidences).Include(p => p.SubRhaimages).Include(o=>o.TindakLanjuts)
                .ThenInclude(p=>p.TindakLanjutEvidences).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByRhaID(string idRha)
        {
            var result = await _db.SubRhas.Include(e => e.SubRhaevidences).Include(i=>i.SubRhaimages).Include(c => c.TindakLanjuts).ThenInclude(p=>p.TindakLanjutEvidences).OrderByDescending(s => s.CreatedAt).Where(s => s.RhaId == Convert.ToInt32(idRha)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByRhaIDandAssign(string idRha, string assign)
        {
            var result = await _db.SubRhas.Include(p=>p.SubRhaimages).Include(e => e.SubRhaevidences).Include(c => c.TindakLanjuts).ThenInclude(o=>o.TindakLanjutEvidences).OrderByDescending(s => s.CreatedAt).Where(s => s.RhaId == Convert.ToInt32(idRha) && s.Assign == assign).AsNoTracking().ToListAsync();
            return result;
        }

        // NOT NECESSARY
        public async Task Insert(SubRha obj)
        {
            try
            {
                _db.SubRhas.Add(obj);
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
        public async Task Update(string id, SubRha obj)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.DivisiBaru = obj.DivisiBaru;
                    result.UicLama = obj.UicLama;
                    result.UicBaru = obj.UicBaru;
                    result.NamaAudit = obj.NamaAudit;
                    result.Lokasi = obj.Lokasi;
                    result.Nomor = obj.Nomor;
                    result.Masalah = obj.Masalah;
                    result.Pendapat = obj.Pendapat;
                    result.Status = obj.Status;
                    result.JatuhTempo = obj.JatuhTempo;
                    result.TahunTemuan = obj.TahunTemuan;
                    result.UsulClose = obj.UsulClose;
                    result.Assign = obj.Assign;
                    result.OpenClose = obj.OpenClose;
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
