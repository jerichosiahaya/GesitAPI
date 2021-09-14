using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class SubRhaEvidenceDto
    {
        public int Id { get; set; }
        public int? SubRhaId { get; set; }
        public string FileName { get; set; }
        public string Notes { get; set; }
        public string Download { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
    }
}
