using System.Linq.Expressions;

namespace Shared.Integrations.D365.BackgroundJobs.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue<TJob>(
        Expression<Func<TJob, Task>> methodCall);

    string Schedule<TJob>(
        Expression<Func<TJob, Task>> methodCall,
        TimeSpan delay);
}