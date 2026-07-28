using MediatR;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using System.Collections.Generic;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;

public sealed class GetAllDailyDepartmentalLabourSlipsQuery : IRequest<IEnumerable<DailyDepartmentalLabourSlipDto>> { }
