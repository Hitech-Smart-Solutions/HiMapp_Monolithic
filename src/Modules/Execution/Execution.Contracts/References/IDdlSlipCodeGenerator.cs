namespace Himapp.Execution.Contracts.References;

/// <summary>
/// Generates DDL slip codes for a given project. Implementations should ensure codes are
/// project-scoped and incremented based on the last stored DDLSlipCode.
/// </summary>
public interface IDdlSlipCodeGenerator
{
    Task<string> GenerateDDLSlipCodeAsync(int projectId, CancellationToken cancellationToken = default);
}
