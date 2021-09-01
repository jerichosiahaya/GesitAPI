using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ISubRha : ICrud<SubRha>
    {
        Task<IEnumerable<SubRha>> GetByRhaID(string idRha);
        Task<IEnumerable<SubRha>> CountExistingFileNameSubRha(string filename);
    }
}
