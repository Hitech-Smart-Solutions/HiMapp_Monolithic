using MediatR;

namespace Himapp.Execution.Application.Features.ProjectActivities.Commands;

public sealed record DeleteProjectActivityCommand(int Id, int ProjectId) : IRequest<bool>;
