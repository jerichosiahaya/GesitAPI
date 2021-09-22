using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Dtos
{
    public class ResponseMonitoringStatusAll
    {
        // to show the status of all project (use it for monitoring page)
        public int ProjectCount { get; set; }
        public int CompletedCountFromProgo { get; set; }
        public int UncompleteCountFromProgo { get; set; }
        public int CompletedCountFromPercentage { get; set; }
        public int UncompleteCountFromPercentage { get; set; }

    }
}
