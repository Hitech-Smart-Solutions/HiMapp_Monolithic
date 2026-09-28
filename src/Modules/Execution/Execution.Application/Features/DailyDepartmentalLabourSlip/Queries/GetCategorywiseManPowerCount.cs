using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries
{

    public sealed class GetCategorywiseManPowerCount(
       int projectID,
       int contractorID,
       DateTime entryDate
   ) : IRequest<IEnumerable<CategoryWiseManpowerDto>>
    {
        public int ProjectID { get; } = projectID;
        public int ContractorID { get; } = contractorID;
        public DateTime EntryDate { get; } = entryDate;
    }
}
