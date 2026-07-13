namespace Himapp.Admin.Contracts.WorkCategories;

public interface IWorkCategoryLookup
{
    Task<WorkCategorySummary?> FindAsync(long workCategoryId, CancellationToken cancellationToken = default);
}

public sealed record WorkCategorySummary(long WorkCategoryId, string Name);
