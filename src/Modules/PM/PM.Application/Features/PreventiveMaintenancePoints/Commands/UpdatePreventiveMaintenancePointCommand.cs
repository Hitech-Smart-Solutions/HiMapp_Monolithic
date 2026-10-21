using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;

public sealed record UpdatePreventiveMaintenancePointCommand(
    long Id,
    string MaintenancePointName,
    int StatusID,
    int LastModifiedBy) : IRequest<PreventiveMaintenancePointDto?>;
