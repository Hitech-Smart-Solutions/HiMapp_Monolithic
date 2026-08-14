namespace Himapp.Execution.Application.Features;

/// <summary>
/// A JSON-safe, typed page of query results.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount);
