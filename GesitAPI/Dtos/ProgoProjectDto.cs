using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class ProgoProjectDto
    {
        public class RootProgoProject
        {
            public List<MappingProject> data { get; set; }
        }

        public class MappingProject
        {
            public string AIPId { get; set; }
            public string NamaAIP { get; set; }
            public string ProjectId { get; set; }
            public string NamaProject { get; set; }
            public string ProjectBudget { get; set; }
            public string ProjectDemandValue { get; set; }
            public string StrategicImportance { get; set; }
            public string Durasi { get; set; }
            public string EksImplementasi { get; set; }
            public string Divisi { get; set; }
            public string LOB { get; set; }
            public string NamaLOB { get; set; }
            public string Squad { get; set; }
            public string NamaSquad { get; set; }
            public string TahunCreate { get; set; }
            public string PeriodeAIP { get; set; }
            public string AplikasiTerdampak { get; set; }
            public string ProjectCategory { get; set; }
            public string JenisPengembangan { get; set; }
            public string Pengembang { get; set; }
            public string PPJTIPihakTerkait { get; set; }
            public string LokasiDC { get; set; }
            public string LokasiDRC { get; set; }
            public string EstimasiBiayaCapex { get; set; }
            public string EstimasiBiayaOpex { get; set; }
            public string statusAIP { get; set; }
        }
    }
}
