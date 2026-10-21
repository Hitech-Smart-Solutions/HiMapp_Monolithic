using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;

public sealed record CreatePreventiveMaintenancePointCommand(
    string MaintenancePointName,
    int StatusID,
    int CreateBy,
    int LastModifiedBy) : IRequest<PreventiveMaintenancePointDto>;
