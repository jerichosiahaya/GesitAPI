using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Models
{
    public class DocumentanddetailsMap
    {
        public int id { get; set; }
        public string aip_id { get; set; }
        public string nama_aip { get; set; }
        public string project_id { get; set; }
        public string project_category { get; set; }
        public string project_title { get; set; }
        public string requirement { get; set; }
        public string cost_benefit_analysis { get; set; }
        public string severity_system { get; set; }
        public string target_implementasi { get; set; }
        public string kategori_project { get; set; }
        public string new_enhance { get; set; }
        public string pengadaan_inhouse { get; set; }
        public string capex_opex { get; set; }
        public string izin_regulator { get; set; }
        public string severity { get; set; }
        public string system_impact { get; set; }
        public string risk { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
