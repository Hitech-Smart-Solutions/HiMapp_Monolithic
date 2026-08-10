using Himapp.Execution.Application.Features.Planning.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Himapp.Execution.Application.Features.Planning.Services.IServices;

public interface IPlanningSectionService
{
    Task<IReadOnlyCollection<PlanningSectionModel>> GetProjectSectionsAsync(int projectId, CancellationToken cancellationToken = default);
}
