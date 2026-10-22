namespace Himapp.PM.Application.Features.WorkDescriptions.Models;

public sealed record WorkDescriptionDto(long Id, string WorkDescriptionName, bool IsActive);

public sealed record WorkDescriptionRequest(string WorkDescriptionName, bool IsActive, int CreateBy, int LastModifiedBy);
