using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries
{
    public sealed record GetDailyLaborContractorsByProjectAndDateQuery(int ProjectId,DateOnly Date) : IRequest<DataSet>;
}
