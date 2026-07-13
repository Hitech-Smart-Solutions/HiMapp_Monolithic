using Himapp.Admin.Contracts.Contractors;
using Himapp.Admin.Contracts.Labour;
using Himapp.Admin.Contracts.Projects;
using Himapp.Admin.Contracts.WorkCategories;

namespace Himapp.Admin.Application.Lookups;

internal sealed class InMemoryAdminLookup : ILabourLookup, IContractorLookup, IProjectDirectory, IWorkCategoryLookup
{
    Task<LabourSummary?> ILabourLookup.FindAsync(long labourId, CancellationToken cancellationToken) =>
        Task.FromResult<LabourSummary?>(new LabourSummary(labourId, 1, "Sample Labour", "InductionPending", 1));

    Task<ContractorSummary?> IContractorLookup.FindAsync(long contractorId, CancellationToken cancellationToken) =>
        Task.FromResult<ContractorSummary?>(new ContractorSummary(contractorId, "Sample Contractor", "9999999999"));

    Task<ProjectSummary?> IProjectDirectory.FindAsync(long projectId, CancellationToken cancellationToken) =>
        Task.FromResult<ProjectSummary?>(new ProjectSummary(projectId, "PRJ-001", "Sample Project"));

    Task<WorkCategorySummary?> IWorkCategoryLookup.FindAsync(long workCategoryId, CancellationToken cancellationToken) =>
        Task.FromResult<WorkCategorySummary?>(new WorkCategorySummary(workCategoryId, "Civil"));
}
