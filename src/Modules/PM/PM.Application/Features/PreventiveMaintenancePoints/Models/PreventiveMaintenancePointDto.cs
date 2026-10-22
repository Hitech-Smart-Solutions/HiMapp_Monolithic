namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;

public sealed record PreventiveMaintenancePointDto(long Id, string? MaintenancePointName, int StatusID);

public sealed record PreventiveMaintenancePointRequest(
    string MaintenancePointName,
    int StatusID,
    int CreateBy,
    int LastModifiedBy);

public sealed record PreventiveMaintenancePointResponse(
    int Id,
    Guid UniqueId,
    string? MaintenancePointName,
    int? StatusID,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt);

public sealed record PreventiveMaintenancePointLookupResponse(
    int Id,
    string? MaintenancePointName);