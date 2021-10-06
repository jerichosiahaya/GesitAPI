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

        // TBC
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SubRha>> GetAll()
        {
            var result = await _db.SubRhas.Include(e => e.SubRhaevidences).Include(c => c.TindakLanjuts).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByAssign(string assign)
        {
            var result = await _db.SubRhas.Where(s => s.Assign == assign).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<SubRha> GetById(string id)
        {
            var result = await _db.SubRhas.Where(s => s.Id == Convert.ToInt32(id)).Include(s=>s.SubRhaevidences).Include(o=>o.TindakLanjuts)
                .ThenInclude(p=>p.TindakLanjutEvidences).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByRhaID(string idRha)
        {
            var result = await _db.SubRhas.Include(e => e.SubRhaevidences).Include(c => c.TindakLanjuts).OrderByDescending(s => s.CreatedAt).Where(s => s.RhaId == Convert.ToInt32(idRha)).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SubRha>> GetByRhaIDandAssign(string idRha, string assign)
        {
            var result = await _db.SubRhas.Include(e => e.SubRhaevidences).Include(c => c.TindakLanjuts).ThenInclude(o=>o.TindakLanjutEvidences).OrderByDescending(s => s.CreatedAt).Where(s => s.RhaId == Convert.ToInt32(idRha) && s.Assign == assign).AsNoTracking().ToListAsync();
            return result;
        }

        // NOT NECESSARY
        public Task Insert(SubRha obj)
        {
            throw new NotImplementedException();
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
