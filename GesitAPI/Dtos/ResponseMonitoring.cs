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
        }
        public string Division { get; set; }
        public float CompletedPercentage { get; set; }
        public int TotalProject { get; set; }
        public int Completed { get; set; }
        public int Uncomplete { get; set; }
        public List<Project> Data { get; set; }

        //public class GroupByDivision
        //{
        //    public string Division { get; set; }
        //    public int Complete { get; set; }
        //    public int Uncomplete { get; set; }
        //    public GroupByDivision()
        //    {
        //        Data = new List<Project>();
        //    }
        //    public List<Project> Data { get; set; }
        //}
        
        public class Project
        {
            public decimal PercentageCompleted { get; set; }
            public string AIPId { get; set; }
            public string NamaAIP { get; set; }
            public string ProjectId { get; set; }
          
        }

    }
}
