using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class InputTlfile
    {
        public InputTlfile()
        {
            InputTlfilesEvidences = new HashSet<InputTlfilesEvidence>();
        }

        public int Id { get; set; }
        public int? RhafilesId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Rhafile Rhafiles { get; set; }
        public virtual ICollection<InputTlfilesEvidence> InputTlfilesEvidences { get; set; }
    }
}
