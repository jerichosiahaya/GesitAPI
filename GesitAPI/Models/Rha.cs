using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class Rha
    {
        public Rha()
        {
            SubRhas = new HashSet<SubRha>();
        }
        public int Id { get; set; }
        public string SubKondisi { get; set; }
        public string Kondisi { get; set; }
        public string Rekomendasi { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Assign { get; set; }
        public string CreatedBy { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusJt { get; set; } // jatuh tempo = MM YYYY
        public string DirSekor { get; set; }
        public string Uic { get; set; }
        public string StatusTemuan { get; set; }
        public virtual ICollection<SubRha> SubRhas { get; set; }
    }
}
