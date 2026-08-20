namespace Himapp.Execution.Contracts.References;

public sealed record UomDto(
    System.Guid UniqueId,
    int Id,
    string UOMCode,
    string UOMName,
    string UOMShortName,
    int ChannelId,
    string OwnerName,
    int CompanyId,
    string? Remarks,
    int StatusId,
    bool IsActive,
    int CreatedBy,
    System.DateTime CreatedDate,
    int LastModifiedBy,
    System.DateTime LastModifiedDate,
    int OrganisationId,
    bool IsDefault,
    string? UOMShortName_EInvoice
);
