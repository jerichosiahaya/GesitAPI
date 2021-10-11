using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class SubRhaImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string ViewImage { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
