using System;
using System.Collections.Generic;

#nullable disable

namespace GesitAPI.Models
{
    public partial class ProgoDokuman
    {
        public int Id { get; set; }
        public string AipId { get; set; }
        public string JenisDokumen { get; set; }
        public string TaksId { get; set; }
        public string UrlDownloadFile { get; set; }

        public virtual ProgoProject Aip { get; set; }
    }
}
