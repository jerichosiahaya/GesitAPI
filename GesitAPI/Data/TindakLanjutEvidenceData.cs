using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public class TindakLanjutEvidenceData : ITindakLanjutEvidence
    {
        public Task Delete(string id)
        {
            throw new NotImplementedException(); 
        }

        public Task<IEnumerable<TindakLanjutEvidence>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TindakLanjutEvidence> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TindakLanjut>> GetByTindakLanjutID(string idTL)
        {
            throw new NotImplementedException();
        }

        public Task Insert(TindakLanjutEvidence obj)
        {
            throw new NotImplementedException();
        }

        public Task Update(string id, TindakLanjutEvidence obj)
        {
            throw new NotImplementedException();
        }
    }
}
