using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsSyncLogs.Models;
using Himapp.PM.Application.Features.DynamicsSyncLogs.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.DynamicsSyncLogs.Handlers;

internal sealed class GetDynamicsSyncLogsQueryHandler : IRequestHandler<GetDynamicsSyncLogsQuery, PagedResult<DynamicsSyncLogDto>>
{
    private readonly IPMDbContext _db;
    public GetDynamicsSyncLogsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PagedResult<DynamicsSyncLogDto>> Handle(GetDynamicsSyncLogsQuery query, CancellationToken cancellationToken)
    {
        var p = query.Params;
        var page = Math.Max(1, p.PageIndex);
        var pageSize = Math.Clamp(p.PageSize, 1, 200);
        var filterColumn = string.IsNullOrWhiteSpace(p.FilterColumn) ? null : p.FilterColumn.Trim();
        var filterValue = string.IsNullOrWhiteSpace(p.FilterValue) ? null : p.FilterValue.Trim();

        // No sort column given: newest run first.
        string sortColumn;
        string sortDirection;
        if (string.IsNullOrWhiteSpace(p.SortColumn))
        {
            sortColumn = "StartedAt";
            sortDirection = "desc";
        }
        else
        {
            sortColumn = p.SortColumn.Trim();
            sortDirection = string.Equals(p.SortDirection?.Trim(), "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        }

        try
        {
            // Paging, filtering and sorting are done by the PostgreSQL functions created in the DynamicsSyncLog migration.
            var rows = await _db.Set<DynamicsSyncLog>()
                .FromSqlInterpolated($"SELECT * FROM \"PAndM\".uspgetdynamicssynclogs({filterColumn}::text, {filterValue}::text, {page}, {pageSize}, {sortColumn}::text, {sortDirection}::text)")
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var total = await _db.Database
                .SqlQuery<long>($"SELECT cnt AS \"Value\" FROM \"PAndM\".uspgetdynamicssynclogscount({filterColumn}::text, {filterValue}::text)")
                .SingleAsync(cancellationToken);

            return new PagedResult<DynamicsSyncLogDto>(rows.Select(ToDto).ToArray(), page, pageSize, (int)total);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "22023")
        {
            // Raised by the function when the filter column does not exist.
            throw new ArgumentException(ex.MessageText);
        }
    }

    private static DynamicsSyncLogDto ToDto(DynamicsSyncLog e) => new()
    {
        Id = e.ID,
        UniqueID = e.UniqueID,
        EntityName = e.EntityName,
        SyncMode = e.SyncMode,
        TriggerSource = e.TriggerSource,
        JobId = e.JobId,
        Status = e.Status,
        StartedAt = e.StartedAt,
        CompletedAt = e.CompletedAt,
        DurationSeconds = e.CompletedAt is null ? null : Math.Round((e.CompletedAt.Value - e.StartedAt).TotalSeconds, 1),
        RecordsFetched = e.RecordsFetched,
        RecordsInserted = e.RecordsInserted,
        RecordsUpdated = e.RecordsUpdated,
        RecordsReset = e.RecordsReset,
        Message = e.Message,
        ErrorDetails = e.ErrorDetails,
        CreatedBy = e.CreatedBy,
        CreatedDate = e.CreatedDate,
        LastModifiedBy = e.LastModifiedBy,
        LastModifiedDate = e.LastModifiedDate
    };
}
