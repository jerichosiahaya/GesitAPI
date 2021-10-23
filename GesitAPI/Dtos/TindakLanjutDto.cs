using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class TindakLanjutDto
    {
        public TindakLanjutDto()
        {
            TindakLanjutEvidences = new HashSet<TindakLanjutEvidenceDto>();
        }
        public int Id { get; set; }
        public int? SubRhaId { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<TindakLanjutEvidenceDto> TindakLanjutEvidences { get; set; }
    }
}
