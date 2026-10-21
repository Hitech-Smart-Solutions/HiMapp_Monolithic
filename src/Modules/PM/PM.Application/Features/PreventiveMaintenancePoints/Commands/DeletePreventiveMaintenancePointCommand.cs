using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;

public sealed record DeletePreventiveMaintenancePointCommand(long Id) : IRequest<bool>;
