using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class TindakLanjutDto
    {
        internal int Id { get; set; }
        public int? SubRhaId { get; set; }
        public string Notes { get; set; }
    }
}
