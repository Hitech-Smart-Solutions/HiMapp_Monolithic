using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.SpareParts.Commands;
using Himapp.PM.Application.Features.SpareParts.Queries;
using Himapp.PM.Application.Features.SpareParts.Models;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.PM.Application.Features.SpareParts.Handlers;

internal sealed class SparePartCatalogueHandlers :
    IRequestHandler<CreateSparePartCatalogueCommand, SparePartCatalogueDto>,
    IRequestHandler<UpdateSparePartCatalogueCommand, SparePartCatalogueDto?>,
    IRequestHandler<DeleteSparePartCatalogueCommand, bool>,
    IRequestHandler<GetAllSparePartCataloguesQuery, List<object>>,
    IRequestHandler<GetSparePartCatalogueByIdQuery, SparePartCatalogueDto?>,
    IRequestHandler<GetSparepartListQuery, PagedResult<SparePartCatalogueListItemDto>>
{
    private readonly IPMDbContext _db;

    public SparePartCatalogueHandlers(IPMDbContext db) => _db = db;

    public async Task<SparePartCatalogueDto> Handle(CreateSparePartCatalogueCommand request, CancellationToken cancellationToken)
    {
        var entity = new SparePartCatalogue
        {
            EquipmentcategoryID = request.EquipmentcategoryID,
            BrandName = request.BrandName,
            ModelName = request.ModelName,
            DocumentName = request.DocumentName,
            DocumentPath = request.DocumentPath,
            StatusID = request.StatusID,
            IsActive = true,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<SparePartCatalogue>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new SparePartCatalogueDto(
            entity.ID,
            request.EquipmentcategoryID,
            request.BrandName,
            request.ModelName,
            request.DocumentName,
            request.DocumentPath,
            request.StatusID,
            true,
            request.CreateBy,
            request.LastModifiedBy);
    }

    public async Task<SparePartCatalogueDto?> Handle(UpdateSparePartCatalogueCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<SparePartCatalogue>().FirstOrDefaultAsync(s => s.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.EquipmentcategoryID = request.EquipmentcategoryID;
        entity.BrandName = request.BrandName;
        entity.ModelName = request.ModelName;
        entity.DocumentName = request.DocumentName;
        entity.DocumentPath = request.DocumentPath;
        entity.StatusID = request.StatusID;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new SparePartCatalogueDto(
            entity.ID,
            entity.EquipmentcategoryID,
            entity.BrandName,
            entity.ModelName,
            entity.DocumentName,
            entity.DocumentPath,
            entity.StatusID,
            entity.IsActive,
            entity.CreatedBy,
            entity.LastModifiedBy);
    }

    public async Task<bool> Handle(DeleteSparePartCatalogueCommand request, CancellationToken cancellationToken)
    {
        var actionHistory = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<SparePartCatalogue>()
            .FirstOrDefaultAsync(p => p.ID == actionHistory.ProgramRowId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.IsActive = actionHistory.Actions == Actions.Activated;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<object>> Handle(GetAllSparePartCataloguesQuery request, CancellationToken cancellationToken)
    {
        var catalogues = await _db.Set<SparePartCatalogue>()
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new
            {
                s.ID,
                s.EquipmentcategoryID,
                s.BrandName,
                s.ModelName,
                s.DocumentName,
                s.DocumentPath,
                s.StatusID,
                s.IsActive,
                s.CreatedBy,
                s.LastModifiedBy
            })
            .ToListAsync(cancellationToken);

        return catalogues.Cast<object>().ToList();
    }

    public async Task<SparePartCatalogueDto?> Handle(GetSparePartCatalogueByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.Set<SparePartCatalogue>()
            .AsNoTracking()
            .Where(s => s.ID == request.Id && s.IsActive)
            .Select(s => new SparePartCatalogueDto(
                s.ID,
                s.EquipmentcategoryID,
                s.BrandName,
                s.ModelName,
                s.DocumentName,
                s.DocumentPath,
                s.StatusID,
                s.IsActive,
                s.CreatedBy,
                s.LastModifiedBy))
            .FirstOrDefaultAsync(cancellationToken);

        return dto;
    }

    public async Task<PagedResult<SparePartCatalogueListItemDto>> Handle(GetSparepartListQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageIndex = page - 1;

        var dbContext = _db as DbContext;
        if (dbContext is null)
            throw new InvalidOperationException("IPMDbContext is not a DbContext. Cannot obtain connection string for Npgsql operations.");

        var items = new List<SparePartCatalogueListItemDto>();
        long totalCount = 0;
        var connString = dbContext.Database.GetDbConnection().ConnectionString;

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        // Get count from stored procedure
        using (var cmd = new NpgsqlCommand("SELECT cnt FROM \"PAndM\".uspgetsparepartcataloguecount(@p_filtercolumn, @p_filtervalue, @p_pageindex, @p_pagesize, @p_sortcolumn, @p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10;
            cmd.Parameters.AddWithValue("@p_filtercolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.FilterColumn) ? (object)DBNull.Value : request.FilterColumn);
            cmd.Parameters.AddWithValue("@p_filtervalue", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.FilterValue) ? (object)DBNull.Value : request.FilterValue);
            cmd.Parameters.AddWithValue("@p_pageindex", NpgsqlDbType.Integer, pageIndex);
            cmd.Parameters.AddWithValue("@p_pagesize", NpgsqlDbType.Integer, pageSize);
            cmd.Parameters.AddWithValue("@p_sortcolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.SortColumn) ? (object)DBNull.Value : request.SortColumn);
            cmd.Parameters.AddWithValue("@p_isactive", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.IsActive) ? (object)DBNull.Value : request.IsActive);

            var countResult = await cmd.ExecuteScalarAsync(cancellationToken);
            totalCount = countResult != null && countResult != DBNull.Value ? Convert.ToInt64(countResult) : 0;
        }

        // Get data from stored procedure
        using (var cmd2 = new NpgsqlCommand("SELECT * FROM \"PAndM\".uspgetsparepartcatalogue(@p_filtercolumn, @p_filtervalue, @p_pageindex, @p_pagesize, @p_sortcolumn, @p_isactive)", conn))
        {
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandTimeout = 30;
            cmd2.Parameters.AddWithValue("@p_filtercolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.FilterColumn) ? (object)DBNull.Value : request.FilterColumn);
            cmd2.Parameters.AddWithValue("@p_filtervalue", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.FilterValue) ? (object)DBNull.Value : request.FilterValue);
            cmd2.Parameters.AddWithValue("@p_pageindex", NpgsqlDbType.Integer, pageIndex);
            cmd2.Parameters.AddWithValue("@p_pagesize", NpgsqlDbType.Integer, pageSize);
            cmd2.Parameters.AddWithValue("@p_sortcolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.SortColumn) ? (object)DBNull.Value : request.SortColumn);
            cmd2.Parameters.AddWithValue("@p_isactive", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(request.IsActive) ? (object)DBNull.Value : request.IsActive);

            using var reader = await cmd2.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var item = new SparePartCatalogueListItemDto(
                    ID: reader.GetInt32(reader.GetOrdinal("ID")),
                    EquipmentcategoryID: reader.GetInt32(reader.GetOrdinal("EquipmentcategoryID")),
                    EquipmentCategoryName: reader.IsDBNull(reader.GetOrdinal("EquipmentCategoryName")) ? null : reader.GetString(reader.GetOrdinal("EquipmentCategoryName")),
                    BrandName: reader.IsDBNull(reader.GetOrdinal("BrandName")) ? null : reader.GetString(reader.GetOrdinal("BrandName")),
                    ModelName: reader.IsDBNull(reader.GetOrdinal("ModelName")) ? null : reader.GetString(reader.GetOrdinal("ModelName")),
                    DocumentName: reader.IsDBNull(reader.GetOrdinal("DocumentName")) ? null : reader.GetString(reader.GetOrdinal("DocumentName")),
                    DocumentPath: reader.IsDBNull(reader.GetOrdinal("DocumentPath")) ? null : reader.GetString(reader.GetOrdinal("DocumentPath")),
                    StatusID: reader.GetInt32(reader.GetOrdinal("StatusID")),
                    IsActive: reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    UniqueID: reader.GetGuid(reader.GetOrdinal("UniqueID")),
                    CreatedDate: reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CreatedBy: reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                    LastModifiedDate: reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")),
                    LastModifiedBy: reader.IsDBNull(reader.GetOrdinal("LastModifiedBy")) ? null : reader.GetInt32(reader.GetOrdinal("LastModifiedBy")));

                items.Add(item);
            }
        }

        return new PagedResult<SparePartCatalogueListItemDto>(
            items,
            page,
            pageSize,
            (int)totalCount);
    }
}
