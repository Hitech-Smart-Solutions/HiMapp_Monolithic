using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using DDLSEntity = Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Services;

internal sealed class DdlSlipCodeGenerator : IDdlSlipCodeGenerator
{
    private readonly IExecutionDbContext _db;
    private readonly IReferenceLookupService? _referenceLookup;
    private readonly Microsoft.Extensions.Logging.ILogger<DdlSlipCodeGenerator> _logger;

    public DdlSlipCodeGenerator(IExecutionDbContext db, IReferenceLookupService? referenceLookup = null, Microsoft.Extensions.Logging.ILogger<DdlSlipCodeGenerator>? logger = null)
    {
        _db = db;
        _referenceLookup = referenceLookup;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DdlSlipCodeGenerator>.Instance;
    }

    public async Task<string> GenerateDDLSlipCodeAsync(int projectId, CancellationToken cancellationToken = default)
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

        // Get last DDLSlipCode for project from Execution DB
        var lastLogCode = await _db.Set<DDLSEntity>()
            .AsNoTracking()
            .Where(l => l.ProjectID == projectId)
            .OrderByDescending(l => l.ID)
            .Select(l => l.DDLSlipCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(projectCode))
        {
            // If project code is unavailable, we cannot generate a meaningful code
            _logger.LogWarning("Cannot generate DDLSlipCode because ProjectCode was not found for ProjectId {ProjectId}", projectId);
            return string.Empty;
        }

        int nextNumber = 1;
        if (!string.IsNullOrWhiteSpace(lastLogCode))
        {
            var prefix = $"DDLS-{projectCode}-";

            if (lastLogCode.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var numberPart = lastLogCode[prefix.Length..];

                if (int.TryParse(numberPart, out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
                else
                {
                    _logger.LogWarning(
                        "Unable to parse numeric part '{NumberPart}' from DDLSlipCode '{LastLogCode}' for ProjectId {ProjectId}",
                        numberPart,
                        lastLogCode,
                        projectId);
                }
            }
            else
            {
                _logger.LogWarning(
                    "Last DDLSlipCode '{LastLogCode}' does not match expected prefix '{Prefix}' for ProjectId {ProjectId}",
                    lastLogCode,
                    prefix,
                    projectId);
            }
        }

        // Format: DDLS-(ProjectCode)-0001 (4 digits)
        var generated = $"DDLS-{projectCode}-{nextNumber:D4}";
        _logger.LogInformation("Generated DDLSlipCode '{Code}' for ProjectId {ProjectId} (last: '{LastLogCode}')", generated, projectId, lastLogCode);
        return generated;
    }
}
