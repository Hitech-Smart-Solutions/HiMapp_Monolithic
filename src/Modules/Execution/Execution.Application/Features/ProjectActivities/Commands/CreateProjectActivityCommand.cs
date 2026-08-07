using MediatR;
using Himapp.Execution.Application.Features.ProjectActivities.Models;

namespace Himapp.Execution.Application.Features.ProjectActivities.Commands;

public sealed record CreateProjectActivityCommand(CreateProjectActivityRequest Request) : IRequest<ProjectActivityModel>;
