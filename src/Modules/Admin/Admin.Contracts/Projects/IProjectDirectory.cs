namespace Himapp.Admin.Contracts.Projects;

public interface IProjectDirectory
{
    Task<ProjectSummary?> FindAsync(long projectId, CancellationToken cancellationToken = default);
}

public sealed record ProjectSummary(long ProjectId, string Code, string Name);
