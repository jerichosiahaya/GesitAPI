using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ISubRhaImage : ICrud<SubRhaimage>
    {
        Task<IEnumerable<SubRhaimage>> CountExistingFileNameSubRhaImage(string filename);
    }
}
