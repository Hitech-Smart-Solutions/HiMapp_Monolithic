using System.Linq.Expressions;
using Hangfire;
using Shared.Integrations.D365.BackgroundJobs.Interfaces;

namespace Shared.Integrations.D365.BackgroundJobs.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    public string Enqueue<TJob>(
        Expression<Func<TJob, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule<TJob>(
        Expression<Func<TJob, Task>> methodCall,
        TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }
}