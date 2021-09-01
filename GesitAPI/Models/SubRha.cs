using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class SubRha
    {
        public SubRha()
        {
            SubRhaevidences = new HashSet<SubRhaevidence>();
            TindakLanjuts = new HashSet<TindakLanjut>();
        }

        public int Id { get; set; }
        public int? RhaId { get; set; }
        public string DivisiBaru { get; set; }
        public string UicBaru { get; set; }
        public string NamaAudit { get; set; }
        public string Lokasi { get; set; }
        public int Nomor { get; set; }
        public string Masalah { get; set; }
        public string Pendapat { get; set; }
        public string Status { get; set; }
        public DateTime JatuhTempo { get; set; }
        public int TahunTemuan { get; set; }
        public string Assign { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Rha Rha { get; set; }
        public virtual ICollection<SubRhaevidence> SubRhaevidences { get; set; }
        public virtual ICollection<TindakLanjut> TindakLanjuts { get; set; }
    }
}
