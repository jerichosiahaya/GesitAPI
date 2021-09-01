using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Data
{
    public interface ISubRhaEvidence : ICrud<SubRhaevidence>
    {
        Task<IEnumerable<SubRhaevidence>> CountExistingFileNameSubRhaEvidence(string filename);
    }
}
