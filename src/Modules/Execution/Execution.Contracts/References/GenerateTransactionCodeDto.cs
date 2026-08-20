namespace Himapp.Execution.Contracts.References;

public sealed record GenerateTransactionCodeDto(
    string? ProjectCode,
    string? LastTransactionCode
);
