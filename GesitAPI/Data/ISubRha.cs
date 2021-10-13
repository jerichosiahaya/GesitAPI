using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ISubRha : ICrud<SubRha>
    {
        Task<IEnumerable<SubRha>> GetAllTracking();
        Task<IEnumerable<SubRha>> GetByRhaID(string idRha);
        Task<IEnumerable<SubRha>> CountExistingFileNameSubRha(string filename);
        Task<IEnumerable<SubRha>> GetByAssign(string assign);
        Task<IEnumerable<SubRha>> GetByRhaIDandAssign(string idRha, string assign);
    }
}
