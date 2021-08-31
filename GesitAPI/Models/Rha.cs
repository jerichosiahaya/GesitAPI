using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class Rha
    {
        public Rha()
        {
            Rhaevidences = new HashSet<Rhaevidence>();
            TindakLanjuts = new HashSet<TindakLanjut>();
        }

        public int Id { get; set; }
        public string UicLama { get; set; }
        public string DivisiBaru { get; set; }
        public string UicBaru { get; set; }
        public string NamaAudit { get; set; }
        public string Lokasi { get; set; }
        public int Nomor { get; set; }
        public string Masalah { get; set; }
        public string Pendapat { get; set; }
        public string Status { get; set; }
        public DateTime JatuhTempo { get; set; }
        public string Assign { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Rhaevidence> Rhaevidences { get; set; }
        public virtual ICollection<TindakLanjut> TindakLanjuts { get; set; }
    }
}
