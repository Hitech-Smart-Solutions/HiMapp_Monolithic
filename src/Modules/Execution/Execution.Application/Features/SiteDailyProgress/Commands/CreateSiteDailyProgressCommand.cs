using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Commands;

public sealed class CreateSiteDailyProgressCommand : IRequest<SiteDailyProgressModel>
{
    public CreateSiteDailyProgressRequest Request { get; }
    public CreateSiteDailyProgressCommand(CreateSiteDailyProgressRequest request) => Request = request;
}
