using Himapp.Execution.Application.Features.DailyProgress.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries
{
    public sealed record CheckTransactionLockQuery(int ProgramID, int ProjectID, int SectionID, DateOnly ReportDate) : IRequest<CheckTransactionLockResponseModel>;
}
