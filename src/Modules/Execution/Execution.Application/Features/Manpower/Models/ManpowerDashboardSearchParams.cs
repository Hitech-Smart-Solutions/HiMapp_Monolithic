using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.Manpower.Models
{
    public sealed class ManpowerDashboardSearchParams
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int[]? ProjectIds { get; set; }
    }
}
