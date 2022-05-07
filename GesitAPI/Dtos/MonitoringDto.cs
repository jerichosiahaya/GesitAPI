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
            public List<DataMonitoring> progoproject { get; set; }
        }
        public class DataMonitoring
        {
            public string StatusProject { get; set; }
            public decimal PercentageCompleted { get; set; }
            public string aip_id { get; set; }
            public string nama_aip { get; set; }
            public string project_id { get; set; }
            public string nama_project { get; set; }
            public string project_budget { get; set; }
            public string project_value { get; set; }
            public string strategic_importance { get; set; }
            public string durasi { get; set; }
            public string eks_implementasi { get; set; }
            public string divisi { get; set; }
            public string lob { get; set; }
            public string nama_lob { get; set; }
            public string squad { get; set; }
            public string nama_squad { get; set; }
            public string tahun_create { get; set; }
            public string periode_aip { get; set; }
            public string aplikasi_terdampak { get; set; }
            public string project_category { get; set; }
            public string jenis_pengembangan { get; set; }
            public string pengembang { get; set; }
            public string ppjti_pihak_terkait { get; set; }
            public string lokasi_dc { get; set; }
            public string lokasi_drc { get; set; }
            public string estimasi_biaya_capex { get; set; }
            public string estimasi_biaya_opex { get; set; }
            public string status_aip { get; set; }
        }
    }
}
