using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Author: Jericho Cristofel Siahaya
// Created: 2021-09-16

namespace GesitAPI.Dtos
{
    public class MonitoringDto
    {
        public class Monitoring
        {
            public List<DataMonitoring> data { get; set; }
        }
        public class DataMonitoring
        {
            public string StatusProject { get; set; }
            public decimal PercentageCompleted { get; set; }
            public string AIPId { get; set; }
            public string NamaAIP { get; set; }
            public string ProjectId { get; set; }
            public string NamaProject { get; set; }
            public string ProjectBudget { get; set; }
            public string ProjectValue { get; set; }
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
