using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface IRha : ICrud<Rha>
    {
        Task<IEnumerable<Rha>> CountExistingFileNameRha(string filename);
        IEnumerable<Rha> GetSubRHAByAssign(string assign);
        Task DeleteAll(string id);
    }
}
