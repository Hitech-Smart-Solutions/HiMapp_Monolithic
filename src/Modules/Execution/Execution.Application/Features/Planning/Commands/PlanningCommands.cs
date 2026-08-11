using MediatR;
using Himapp.Execution.Application.Features.Planning.Models;

namespace Himapp.Execution.Application.Features.Planning.Commands;

public sealed record CreatePlanningCommand(CreatePlanningRequest Request) : IRequest<PlanningModel>;
public sealed record UpdatePlanningCommand(int Id, UpdatePlanningRequest Request) : IRequest<PlanningModel?>;
public sealed record DeletePlanningCommand(int Id, int DeletedBy) : IRequest<bool>;
public sealed record BulkCreatePlanningCommand(BulkCreatePlanningRequest Request) : IRequest<IReadOnlyCollection<PlanningModel>>;
