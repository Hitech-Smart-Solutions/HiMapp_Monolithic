namespace Himapp.Execution.Application.Features.Activities;

public sealed record ActivityDto(long Id, long ProjectId, string ActivityCode, string Description, decimal ProgressPercent, DateOnly WorkDate);

public sealed record ActivityRequest(long ProjectId, string ActivityCode, string Description, decimal ProgressPercent, DateOnly WorkDate);
