using System.Threading;
using System.Threading.Tasks;
using Himapp.Execution.Application.Features.Planning.Models;
using Microsoft.AspNetCore.Http;

namespace Himapp.Execution.Application.Features.Planning.Services;

public interface IExcelPlanningImporter
{
    Task<PlanningImportParseResult> ParseAsync(IFormFile file, int projectId, CancellationToken cancellationToken = default);
}
