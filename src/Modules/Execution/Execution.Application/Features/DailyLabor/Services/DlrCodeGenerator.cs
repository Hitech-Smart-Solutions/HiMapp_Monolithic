using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Himapp.Execution.Application.Features.DailyLabor.Services;

internal sealed class DlrCodeGenerator : IDlrCodeGenerator
{
    private readonly IExecutionDbContext _db;
    private readonly Himapp.Execution.Contracts.References.IReferenceLookupService? _referenceLookup;
    private readonly ILogger<DlrCodeGenerator> _logger;

    public DlrCodeGenerator(IExecutionDbContext db, Himapp.Execution.Contracts.References.IReferenceLookupService? referenceLookup = null, ILogger<DlrCodeGenerator>? logger = null)
    {
        _db = db;
        _referenceLookup = referenceLookup;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DlrCodeGenerator>.Instance;
    }

    public async Task<string> GenerateDLRCodeAsync(int projectId, CancellationToken cancellationToken = default)
    {
        string? projectCode = null;
        if (_referenceLookup is not null)
        {
            var project = await _referenceLookup.GetProjectAsync(projectId, cancellationToken);
            projectCode = project?.ProjectCode;
            _logger.LogDebug("PublicSchema lookup for ProjectId {ProjectId} returned ProjectCode '{ProjectCode}'", projectId, projectCode);
        }

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

        var lastCode = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .AsNoTracking()
            .Where(l => l.ProjectID == projectId)
            .OrderByDescending(l => l.ID)
            .Select(l => l.DLRCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(projectCode))
        {
            _logger.LogWarning("Cannot generate DLRCode because ProjectCode was not found for ProjectId {ProjectId}", projectId);
            return string.Empty;
        }

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCode))
        {
            // Expecting format: DLR-(ProjectCode)-0001
            var prefix = $"DLR-{projectCode}-";
            var lastNumberPart = lastCode.Replace(prefix, "");
            if (int.TryParse(lastNumberPart, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
            else
            {
                _logger.LogWarning("Unable to parse last DLRCode '{LastCode}' for ProjectId {ProjectId}", lastCode, projectId);
            }
        }

        var generated = $"DLR-({projectCode})-{nextNumber:D4}";
        _logger.LogInformation("Generated DLRCode '{Code}' for ProjectId {ProjectId} (last: '{LastCode}')", generated, projectId, lastCode);
        return generated;
    }
}
