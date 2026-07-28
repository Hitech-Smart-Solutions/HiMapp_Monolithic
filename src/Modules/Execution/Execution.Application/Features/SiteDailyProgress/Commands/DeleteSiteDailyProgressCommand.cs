using MediatR;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Commands;

public sealed class DeleteSiteDailyProgressCommand : IRequest<bool>
{
    public int Id { get; }
    public DeleteSiteDailyProgressCommand(int id) => Id = id;
}
