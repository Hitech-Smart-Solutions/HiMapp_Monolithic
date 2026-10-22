namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;

public sealed record PreventiveMaintenancePointDto(long Id, string? MaintenancePointName, int StatusID);

public sealed record PreventiveMaintenancePointRequest(
    string? MaintenancePointName,
    int StatusID,
    int CreateBy,
    int LastModifiedBy);
