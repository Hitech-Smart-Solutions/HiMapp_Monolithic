using Himapp.Execution.Contracts.DailyLabor;
using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Services;

internal sealed class DailyLaborService : IDailyLaborService
{
    private readonly IExecutionDbContext _db;
    private readonly IReferenceLookupService? _referenceLookup;
    private readonly ILogger<DailyLaborService> _logger;

    public DailyLaborService(IExecutionDbContext db, IReferenceLookupService? referenceLookup = null, ILogger<DailyLaborService>? logger = null)
    {
        _db = db;
        _referenceLookup = referenceLookup;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DailyLaborService>.Instance;
    }

    public async Task<IReadOnlyCollection<DPRDailyLaborConsolidatedModel>>GetDPRConsolidatedDailyLaborAsync(int projectId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var dbContext = _db as DbContext;

        if (dbContext is null)
        {
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext.");
        }

        var connection = dbContext.Database.GetDbConnection();

        const string sql = """
            SELECT
                d."ContractorID" AS "ContractorID",
                d."ContractorName" AS "ContractorName",
                d."ActivityID" AS "ActivityID",
                a."ActivityName" AS "ActivityName",

                COALESCE(SUM(d."Skilled"), 0)::integer AS "Skilled",
                COALESCE(SUM(d."UnSkilled"), 0)::integer AS "Unskilled",
                COALESCE(SUM(d."Mat"), 0)::integer AS "Mat",

                COALESCE(
                    SUM(
                        COALESCE(d."Skilled", 0) +
                        COALESCE(d."UnSkilled", 0) +
                        COALESCE(d."Mat", 0)
                    ),
                    0
                )::integer AS "Total"

            FROM execution."DailyLabor" dl

            INNER JOIN execution."DailyLaborDetails" d
                ON d."DailyLabourID" = dl."ID"

            INNER JOIN execution."Activities" a
                ON a."ID" = d."ActivityID"

            WHERE
                dl."ProjectID" = @ProjectId
                AND dl."ReportDate" >= @StartDate
                AND dl."ReportDate" < @EndDate
                AND dl."IsActive" = TRUE
                AND d."IsActive" = TRUE

            GROUP BY
                d."ContractorID",
                d."ContractorName",
                d."ActivityID",
                a."ActivityName"

            ORDER BY
                d."ContractorName",
                a."ActivityName";
            """;

        var startDate = date.ToDateTime(TimeOnly.MinValue);
        var endDate = startDate.AddDays(1);

        try
        {
            await connection.OpenAsync(cancellationToken);

            using var cmd = connection.CreateCommand();

            cmd.CommandText = sql;

            var projectIdParameter = cmd.CreateParameter();
            projectIdParameter.ParameterName = "@ProjectId";
            projectIdParameter.Value = projectId;
            cmd.Parameters.Add(projectIdParameter);

            var startDateParameter = cmd.CreateParameter();
            startDateParameter.ParameterName = "@StartDate";
            startDateParameter.Value = startDate;
            cmd.Parameters.Add(startDateParameter);

            var endDateParameter = cmd.CreateParameter();
            endDateParameter.ParameterName = "@EndDate";
            endDateParameter.Value = endDate;
            cmd.Parameters.Add(endDateParameter);

            var result = new List<DPRDailyLaborConsolidatedModel>();

            using var reader =
                await cmd.ExecuteReaderAsync(cancellationToken);

            var contractorIdOrdinal = reader.GetOrdinal("ContractorID");
            var contractorNameOrdinal = reader.GetOrdinal("ContractorName");
            var activityIdOrdinal = reader.GetOrdinal("ActivityID");
            var activityNameOrdinal = reader.GetOrdinal("ActivityName");
            var skilledOrdinal = reader.GetOrdinal("Skilled");
            var unskilledOrdinal = reader.GetOrdinal("Unskilled");
            var matOrdinal = reader.GetOrdinal("Mat");
            var totalOrdinal = reader.GetOrdinal("Total");

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new DPRDailyLaborConsolidatedModel(
                    reader.IsDBNull(contractorIdOrdinal)
                        ? null
                        : reader.GetInt32(contractorIdOrdinal),

                    reader.IsDBNull(contractorNameOrdinal)
                        ? null
                        : reader.GetString(contractorNameOrdinal),

                    reader.IsDBNull(activityIdOrdinal)
                        ? null
                        : reader.GetInt32(activityIdOrdinal),

                    reader.IsDBNull(activityNameOrdinal)
                        ? null
                        : reader.GetString(activityNameOrdinal),

                    reader.GetInt32(skilledOrdinal),
                    reader.GetInt32(unskilledOrdinal),
                    reader.GetInt32(matOrdinal),
                    reader.GetInt32(totalOrdinal)
                ));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting consolidated daily labor for ProjectId {ProjectId}, Date {Date}",
                projectId,
                date);

            throw;
        }
    }
}
