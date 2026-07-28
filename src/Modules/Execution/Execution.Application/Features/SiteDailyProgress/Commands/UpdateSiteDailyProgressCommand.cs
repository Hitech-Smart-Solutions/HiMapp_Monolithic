using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Commands;

public sealed class UpdateSiteDailyProgressCommand : IRequest<SiteDailyProgressDto?>
{
    public int Id { get; }
    public UpdateSiteDailyProgressRequest Request { get; }
    public UpdateSiteDailyProgressCommand(int id, UpdateSiteDailyProgressRequest request)
    {
        Id = id;
        Request = request;
    }
}
