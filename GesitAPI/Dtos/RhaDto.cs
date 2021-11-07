using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class RhaDto
    {
        public RhaDto()
        {
            StatusInfo = new List<StatusInfoRha>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
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
        public List<StatusInfoRha> StatusInfo { get; set; }

        public class StatusInfoRha
        {
            public int CountSubRha { get; set; }
            public int CountSubRHAClosed { get; set; }
            public int CountSubRHAOpen { get; set; }
            public float StatusCompletedPercentage { get; set; }

        }

    }

    public class ResponseStatusRha
    {
        public int CountRha { get; set; }
        public int CompletedRha { get; set; }
        public int UncompleteRha { get; set; }
        public float PercentageCompletedRha { get; set; }
        public float PercentageUncompleteRha { get; set; }
    }

}
