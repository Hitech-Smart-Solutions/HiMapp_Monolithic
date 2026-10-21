namespace Himapp.PM.Application.Features.Common;

/// <summary>
/// Common list settings sent by the frontend (query string): one-column filter, sorting and paging.
/// PageIndex starts at 1.
/// </summary>
public sealed class ListSearchParams
{
    /// <summary>Column to filter on. When empty, FilterValue is searched across the default search columns.</summary>
    public string? FilterColumn { get; set; }
    public string? FilterValue { get; set; }
    public string? SortColumn { get; set; }
    /// <summary>"asc" (default) or "desc".</summary>
    public string? SortDirection { get; set; } = "asc";
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
