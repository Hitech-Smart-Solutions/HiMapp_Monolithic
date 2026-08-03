using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries;

public sealed class GetSiteDailyProgressByIdQuery : IRequest<SiteDailyProgressModel?>
{
    public int Id { get; }
    public GetSiteDailyProgressByIdQuery(int id) => Id = id;
}
