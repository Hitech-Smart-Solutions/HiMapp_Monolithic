namespace Himapp.Admin.Contracts.Projects;

public interface IProjectDirectory
{
    Task<ProjectSummary?> FindAsync(int projectId, CancellationToken cancellationToken = default);
}

public sealed record ProjectSummary(int ProjectId, string Code, string Name);
