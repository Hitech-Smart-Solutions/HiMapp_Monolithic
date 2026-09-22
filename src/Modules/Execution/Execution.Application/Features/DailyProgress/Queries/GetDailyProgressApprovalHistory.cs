using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries
{
    public sealed record GetDailyProgressApprovalHistory(int programId, int id) : IRequest<DataSet>;
}
