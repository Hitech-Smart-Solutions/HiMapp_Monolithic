using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries
{
    public class GetActivityWiseQuantityBySectionIDQuery : IRequest<List<ActivityWiseQuantityBySectionModel>>
    {
        public int AreaID { get; set; }
        public int ProjectID { get; set; }
        public DateOnly ReportDate { get; set; }
    }
}
