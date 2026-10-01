using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Contracts.DailyLabor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries;

public sealed record DPRGetConsolidatedDailyLaborQuery(DateOnly Date, int ProjectId,int Id) : IRequest<DPRDailyLaborConsolidatedResponse>;
