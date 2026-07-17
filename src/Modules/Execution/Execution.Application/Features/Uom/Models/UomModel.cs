namespace Himapp.Execution.Application.Features.Uom.Models;

public sealed record UomModel(
    long Id,
    System.Guid UniqueId,
    long CompanyId,
    string Name,
    string Code,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateUomRequest(long CompanyId, string Name, string Code);
public sealed record UpdateUomRequest(string Name, string Code, bool IsActive);
