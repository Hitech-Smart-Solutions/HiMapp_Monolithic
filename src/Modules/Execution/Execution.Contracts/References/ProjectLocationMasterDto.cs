namespace Himapp.Execution.Contracts.References;

public sealed record ProjectLocationMasterDto(
    int ID,
    System.Guid UniqueID,
    string? LocationCode,
    string? LocationName,
    int ProjectID,
    int? CompanyID,
    string? Remarks,
    bool? IsActive,
    int CreatedBy,
    System.DateTime CreatedDate,
    int LastModifiedBy,
    System.DateTime LastModifiedDate,
    int? OrganizationID,
    int? StatusID
);
