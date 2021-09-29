using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectCategory { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectDocument { get; set; }
        public DateTime TargetDate { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedFor { get; set; }
        public int Status { get; set; }


        public class NotificationInsert
        {
            public string ProjectId { get; set; }
            public string ProjectCategory { get; set; }
            public string ProjectTitle { get; set; }
            public string ProjectDocument { get; set; }
            public DateTime TargetDate { get; set; }
            public string AssignedBy { get; set; }
            public string AssignedFor { get; set; }
            public int Status { get; set; }
        }

    }
}
