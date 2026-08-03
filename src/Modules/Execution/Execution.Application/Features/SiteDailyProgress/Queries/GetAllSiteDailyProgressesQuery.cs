using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using System.Collections.Generic;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries;

public sealed class GetAllSiteDailyProgressesQuery : IRequest<IEnumerable<SiteDailyProgressModel>> { }
