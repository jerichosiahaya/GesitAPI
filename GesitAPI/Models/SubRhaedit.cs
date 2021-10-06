using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Models
{
    public class SubRhaedit
    {
        public string DivisiBaru  { get; set; }
        public string UICBaru { get; set; }
        public string UicLama { get; set; } // new added
        public string NamaAudit { get; set; }
        public string Masalah { get; set; }
        public string Pendapat { get; set; }
        public DateTime? Status { get; set; }
        public DateTime? UsulanClose { get; set; }
        public DateTime? JatuhTempo { get; set; }
        public DateTime? TahunTemuan { get; set; }
        public string Assign { get; set; }
        public string UsulClose { get; set; } // new added
        public int OpenClose { get; set; } // new added
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
