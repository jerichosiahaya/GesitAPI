using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ITindakLanjutEvidence : ICrud<TindakLanjutEvidence>
    {
        Task<IEnumerable<TindakLanjutEvidence>> GetByTindakLanjutID(string idTL);
        Task<IEnumerable<TindakLanjutEvidence>> CountExistingFileNameTLEvidence(string filename);
    }
}
