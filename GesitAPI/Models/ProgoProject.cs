using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class ProgoProject
    {
        public ProgoProject()
        {
            ProgoDokumen = new HashSet<ProgoDokuman>();
        }

        public string AipId { get; set; }
        public string NamaAip { get; set; }
        public string ProjectId { get; set; }
        public string NamaProject { get; set; }
        public long? ProjectBudget { get; set; }
        public long? ProjectDemandValue { get; set; }
        public string StrategicImportance { get; set; }
        public int? Durasi { get; set; }
        public string EksImplementasi { get; set; }
        public string Divisi { get; set; }
        public int? Lob { get; set; }
        public string NamaLob { get; set; }
        public string Squad { get; set; }
        public string NamaSquad { get; set; }
        public int? TahunCreate { get; set; }
        public int? PeriodeAip { get; set; }
        public string AplikasiTerdampak { get; set; }
        public string ProjectCategory { get; set; }
        public string JenisPengembangan { get; set; }
        public string Pengembang { get; set; }
        public string PpjtiPihakTerkait { get; set; }
        public string LokasiDc { get; set; }
        public string LokasiDrc { get; set; }
        public long? EstimasiBiayaCapex { get; set; }
        public long? EstimasiBiayaOpex { get; set; }
        public string StatusAip { get; set; }

        public virtual ICollection<ProgoDokuman> ProgoDokumen { get; set; }
    }
}
