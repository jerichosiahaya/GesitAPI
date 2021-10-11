using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class NotificationDto
    {

        public class IsoDateConverter : IsoDateTimeConverter
        {
            public IsoDateConverter() =>
                this.DateTimeFormat = Culture.DateTimeFormat.ShortDatePattern;
        }

        public class NotificationView
        {
            public int Id { get; set; }
            public string ProjectId { get; set; }
            public string ProjectCategory { get; set; }
            public string ProjectTitle { get; set; }
            public string ProjectDocument { get; set; }
            public string TargetDate { get; set; }
            public string AssignedBy { get; set; }
            public string AssignedFor { get; set; }
            public int Status { get; set; }
        }
        
        public class NotificationInsert
        {
            public string ProjectId { get; set; }
            public string ProjectCategory { get; set; }
            public string ProjectTitle { get; set; }
            public string ProjectDocument { get; set; }
            [DataType(DataType.Date)]
            public DateTime TargetDate { get; set; }
            public string AssignedBy { get; set; }
            public string AssignedFor { get; set; }
            public int Status { get; set; }
        }

    }
}
