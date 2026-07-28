using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries;

public sealed class GetSiteDailyProgressByIdQuery : IRequest<SiteDailyProgressDto?>
{
    public int Id { get; }
    public GetSiteDailyProgressByIdQuery(int id) => Id = id;
}
