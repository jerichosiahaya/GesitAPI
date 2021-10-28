using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class ProgoDocumentDto
    {
        public class RootProgoDocument
        {
            public List<MappingDocument> data { get; set; }
        }

        public class MappingDocument
        {
            public int Id { get; set; }
            public string AipId { get; set; }
            public string JenisDokumen { get; set; }
            public string TaskId { get; set; }
            public int? Tahun { get; set; }
            public string NamaFile { get; set; }
            public string UrlDownloadFile { get; set; }
        }
    }
}
