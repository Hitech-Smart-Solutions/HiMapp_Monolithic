using MediatR;
using Himapp.Execution.Application.Features.Planning.Models;

namespace Himapp.Execution.Application.Features.Planning.Commands;

public sealed record CreatePlanningCommand(CreatePlanningRequest Request) : IRequest<PlanningModel>;
public sealed record UpdatePlanningCommand(long Id, UpdatePlanningRequest Request) : IRequest<PlanningModel?>;
public sealed record DeletePlanningCommand(long Id) : IRequest<bool>;
