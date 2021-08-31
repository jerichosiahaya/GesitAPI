using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class Rhafile
    {
        public Rhafile()
        {
            InputTlfiles = new HashSet<InputTlfile>();
            RhafilesEvidences = new HashSet<RhafilesEvidence>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string SubKondisi { get; set; }
        public string Kondisi { get; set; }
        public string Rekomendasi { get; set; }
        public string TindakLanjut { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Assign { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? StatusCompleted { get; set; }

        public virtual ICollection<InputTlfile> InputTlfiles { get; set; }
        public virtual ICollection<RhafilesEvidence> RhafilesEvidences { get; set; }
    }
}
