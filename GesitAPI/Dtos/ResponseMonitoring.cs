using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Author: Jericho Cristofel Siahaya
// Created: 2021-09-16

namespace GesitAPI.Dtos
{
    public class ResponseMonitoring
    {
        public ResponseMonitoring()
        {
            Data = new List<Project>();
            Status = new List<StatusInfoMonitoring>();
        }
        public string Division { get; set; }
        public float CompletedPercentage { get; set; }
        public int TotalProject { get; set; }
        //public int Completed { get; set; }
        //public int Uncomplete { get; set; }
        public List<StatusInfoMonitoring> Status { get; set; }
        public List<Project> Data { get; set; }

        public class StatusInfoMonitoring
        {
            public int CompletedFromPercentage { get; set; }
            public int UncompleteFromPercentage { get; set; }
            public int CompletedFromProgo { get; set; }
            public int UncompleteFromProgo { get; set; }
        }
        
        public class Project
        {
            public string StatusProject { get; set; }
            public decimal PercentageCompleted { get; set; }
            public string AIPId { get; set; }
            public string NamaAIP { get; set; }
            public string ProjectId { get; set; }
            public string statusAIP { get; set; }

        }

    }
}
