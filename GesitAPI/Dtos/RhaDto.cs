using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class RhaDto
    {
        internal int Id { get; set; }
        public string SubKondisi { get; set; }
        public string Kondisi { get; set; }
        public string Rekomendasi { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Assign { get; set; }
        public string CreatedBy { get; set; }
        public string StatusJt { get; set; }
        public string DirSekor { get; set; }
        public string Uic { get; set; }
        public string StatusTemuan { get; set; }
    }
}
