using Himapp.Execution.Application.Features.Activities.Models;
using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Queries;

public sealed record GetActivityByIdQuery(int Id) : IRequest<ActivityDto?>;
