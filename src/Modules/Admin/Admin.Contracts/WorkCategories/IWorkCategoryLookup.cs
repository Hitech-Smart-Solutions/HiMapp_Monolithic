namespace Himapp.Admin.Contracts.WorkCategories;

public interface IWorkCategoryLookup
{
    Task<WorkCategorySummary?> FindAsync(int workCategoryId, CancellationToken cancellationToken = default);
}

public sealed record WorkCategorySummary(int WorkCategoryId, string Name);
