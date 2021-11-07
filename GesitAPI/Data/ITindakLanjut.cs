using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ITindakLanjut : ICrud<TindakLanjut>
    {
        Task<IEnumerable<TindakLanjut>> GetBySubRhaID(string idRha);
        Task<IEnumerable<TindakLanjut>> CountExistingFileNameTindakLanjut(string filename);
    }
}
