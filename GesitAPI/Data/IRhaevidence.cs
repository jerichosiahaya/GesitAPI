using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface IRhaevidence : ICrud<Rhaevidence>
    {
        Task<IEnumerable<Rhaevidence>> GetByRhaID(string idRha);
        Task<IEnumerable<Rhaevidence>> CountExistingFileNameRhaEvidence(string filename);
    }
}
