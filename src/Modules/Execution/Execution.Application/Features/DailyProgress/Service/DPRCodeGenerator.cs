using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Services;
using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Service;

internal sealed class DPRCodeGenerator : IDPRCodeGenerator
{
    private readonly IExecutionDbContext _db;
    private readonly IReferenceLookupService? _referenceLookup;
    private readonly Microsoft.Extensions.Logging.ILogger<DPRCodeGenerator> _logger;

    public DPRCodeGenerator(IExecutionDbContext db, IReferenceLookupService? referenceLookup = null, Microsoft.Extensions.Logging.ILogger<DPRCodeGenerator>? logger = null)
    {
        _db = db;
        _referenceLookup = referenceLookup;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DPRCodeGenerator>.Instance;
    }

    public async Task<string> GenerateDPRCodeAsync(int projectId, CancellationToken cancellationToken = default)
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

        // If still null, leave projectCode empty -- generator will return empty string

        // Get last DPRCode for project from Execution DB
        var lastLogCode = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Where(l => l.ProjectID == projectId)
            .OrderByDescending(l => l.ID)
            .Select(l => l.DPRCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(projectCode))
        {
            // If project code is unavailable, we cannot generate a meaningful code
            _logger.LogWarning("Cannot generate DPRCode because ProjectCode was not found for ProjectId {ProjectId}", projectId);
            return string.Empty;
        }

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastLogCode))
        {
            // Expecting format: DPR-(ProjectCode)-0001
            var prefix = $"DPR-{projectCode}-";
            var lastCode = lastLogCode.Replace(prefix, "");
            if (int.TryParse(lastCode, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
            else
            {
                _logger.LogWarning("Unable to parse last DPRCode '{LastLogCode}' for ProjectId {ProjectId}", lastLogCode, projectId);
            }
        }

        // Format: DPR-(ProjectCode)-0001 (4 digits)
        var generated = $"DPR-{projectCode}-{nextNumber:D4}";
        _logger.LogInformation("Generated DPRCode '{Code}' for ProjectId {ProjectId} (last: '{LastLogCode}')", generated, projectId, lastLogCode);
        return generated;
    }
}
