using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class ProgoDocument
    {
        public int Id { get; set; }
        public string AipId { get; set; }
        public string JenisDokumen { get; set; }
        public string TaskId { get; set; }
        public string UrlDownloadFile { get; set; }
        public int? Tahun { get; set; }
        public string NamaFile { get; set; }

        public virtual ProgoProject Aip { get; set; }
    }
}
