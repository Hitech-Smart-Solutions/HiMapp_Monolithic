using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

using LockRequestEntity = Himapp.Execution.Domain.Entities.LockRequest;

namespace Himapp.Execution.Application.Features.LockRequest.Services;

internal sealed class LockRequestCodeGenerator : ILockRequestCodeGenerator
{
    private readonly IExecutionDbContext _db;
    private readonly IReferenceLookupService? _referenceLookup;
    private readonly ILogger<LockRequestCodeGenerator> _logger;

    public LockRequestCodeGenerator(IExecutionDbContext db, IReferenceLookupService? referenceLookup = null, ILogger<LockRequestCodeGenerator>? logger = null)
    {
        _db = db;
        _referenceLookup = referenceLookup;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<LockRequestCodeGenerator>.Instance;
    }

    public async Task<string> GenerateLockRequestCodeAsync(int projectId, CancellationToken cancellationToken = default)
    {
        // Try to obtain project code from the reference lookup service if available
        string? projectCode = null;
        if (_referenceLookup is not null)
        {
            var project = await _referenceLookup.GetProjectAsync(projectId, cancellationToken);
            projectCode = project?.ProjectCode;
            _logger.LogDebug("PublicSchema lookup for ProjectId {ProjectId} returned ProjectCode '{ProjectCode}'", projectId, projectCode);
        }

        // Fallback: try to read ProjectCode directly from public.ProjectMaster using DB connection
        if (string.IsNullOrEmpty(projectCode))
        {
            try
            {
                var conn = _db.Database.GetDbConnection();
                await conn.OpenAsync(cancellationToken);
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT \"ProjectCode\" FROM public.\"ProjectMaster\" WHERE (\"Id\" = @id OR \"ID\" = @id OR id = @id) LIMIT 1";
                var p = cmd.CreateParameter();
                p.ParameterName = "@id";
                p.Value = projectId;
                cmd.Parameters.Add(p);
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                if (result is not null && result != DBNull.Value)
                {
                    projectCode = result.ToString();
                    _logger.LogDebug("Direct DB lookup for ProjectId {ProjectId} returned ProjectCode '{ProjectCode}'", projectId, projectCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Direct DB lookup for ProjectId {ProjectId} failed", projectId);
            }
        }

        // Get last RequestCode for project from Execution DB
        var lastCode = await _db.Set<LockRequestEntity>()
            .AsNoTracking()
            .Where(l => l.ProjectID == projectId)
            .OrderByDescending(l => l.ID)
            .Select(l => l.RequestCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(projectCode))
        {
            _logger.LogWarning("Cannot generate LockRequestCode because ProjectCode was not found for ProjectId {ProjectId}", projectId);
            return string.Empty;
        }

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCode))
        {
            // Expecting format: LR-(ProjectCode)-0001
            var prefix = $"LR-{projectCode}-";
            var lastNumberPart = lastCode.Replace(prefix, "");
            if (int.TryParse(lastNumberPart, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
            else
            {
                _logger.LogWarning("Unable to parse last LockRequestCode '{LastCode}' for ProjectId {ProjectId}", lastCode, projectId);
            }
        }

        // Format: LR-(ProjectCode)-0001 (4 digits)
        var generated = $"LR-{projectCode}-{nextNumber:D4}";
        _logger.LogInformation("Generated LockRequestCode '{Code}' for ProjectId {ProjectId} (last: '{LastCode}')", generated, projectId, lastCode);
        return generated;
    }
}
