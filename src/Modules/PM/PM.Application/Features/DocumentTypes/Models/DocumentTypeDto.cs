namespace Himapp.PM.Application.Features.DocumentTypes.Models;

public sealed record DocumentTypeDto(long Id, string DocumentTypeName);

public sealed record DocumentTypeRequest(string DocumentTypeName, int CreateBy, int LastModifiedBy, string? Remarks);
