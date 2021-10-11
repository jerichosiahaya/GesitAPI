using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class Checklist
    {
        public int Id { get; set; }
        public string AipId { get; set; }
        public string NamaAip { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCategory { get; set; }
        public string ProjectTitle { get; set; }
        public string Requirement { get; set; }
        public string CostBenefitAnalysis { get; set; }
        public string SeveritySystem { get; set; }
        public string TargetImplementasi { get; set; }
        public string KategoriProject { get; set; }
        public string NewEnhance { get; set; }
        public string PengadaanInhouse { get; set; }
        public string CapexOpex { get; set; }
        public string IzinRegulator { get; set; }
        public string Severity { get; set; }
        public string SystemImpact { get; set; }
        public string Risk { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
