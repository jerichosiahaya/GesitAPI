using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class SubRhaDto
    {
        public int Id { get; set; }
        public int RhaId { get; set; }
        public string DivisiBaru { get; set; }
        public string UicLama { get; set; } // new added
        public string UicBaru { get; set; }
        public string NamaAudit { get; set; }
        public string Lokasi { get; set; }
        public int Nomor { get; set; }
        public string Masalah { get; set; }
        public string Pendapat { get; set; }
        public string StatusJatuhTempo { get; set; }
        public string Status { get; set; }
        public int JatuhTempo { get; set; }
        public int TahunTemuan { get; set; }
        public string Assign { get; set; }
        public string UsulClose { get; set; } // new added
        public string OpenClose { get; set; } // new added
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // use it to insert single sub rha
    public class SubRhaInsert
    {
        public int RhaId { get; set; }
        public string DivisiBaru { get; set; }
        public string UicLama { get; set; } // new added
        public string UicBaru { get; set; }
        public string NamaAudit { get; set; }
        public string Lokasi { get; set; }
        public int Nomor { get; set; }
        public string Masalah { get; set; }
        public string Pendapat { get; set; }
        public string StatusJatuhTempo { get; set; }
        public string Status { get; set; }
        public int JatuhTempo { get; set; }
        public int TahunTemuan { get; set; }
        public string Assign { get; set; }
        public string UsulClose { get; set; } // new added
        public string OpenClose { get; set; } // new added
    }

}
