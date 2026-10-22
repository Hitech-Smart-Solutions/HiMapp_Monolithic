namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;

public sealed record PreventiveMaintenancePointListItemDto(
    int ID,
    string? MaintenancePointName,
    int StatusID,
    bool IsActive,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
