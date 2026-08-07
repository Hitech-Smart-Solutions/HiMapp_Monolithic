using MediatR;
using Himapp.Execution.Application.Features.ProjectActivities.Models;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetProjectActivityByIdQuery(int Id) : IRequest<ProjectActivityModel?>;
