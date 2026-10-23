using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.SpareParts.Models;
using Himapp.PM.Application.Features.SpareParts.Queries;
using Himapp.PM.Application.Features.WorkDescriptions.Commands;
using Himapp.PM.Application.Features.WorkDescriptions.Models;
using Himapp.PM.Application.Features.WorkDescriptions.Queries;
using Himapp.PM.Domain.Entities;
using Himapp.Api.src.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.PM.Application.Features.WorkDescriptions.Handlers;

internal sealed class CreateWorkDescriptionCommandHandler : IRequestHandler<CreateWorkDescriptionCommand, WorkDescriptionDto>
{
    private readonly IPMDbContext _db;

    public CreateWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto> Handle(CreateWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkDescriptionName))
                throw new BadRequestException("Work description name is required.", "WORK_DESCRIPTION_NAME_REQUIRED");

            var exists = await _db.Set<WorkDescription>().AsNoTracking()
                .AnyAsync(d => d.WorkDescriptionName.ToLower() == request.WorkDescriptionName.ToLower(), cancellationToken);
            if (exists)
                throw new BadRequestException($"A work description with name '{request.WorkDescriptionName}' already exists.", "WORK_DESCRIPTION_DUPLICATE");

            var entity = new WorkDescription
            {
                UniqueID = Guid.NewGuid(),
                WorkDescriptionName = request.WorkDescriptionName,
                IsActive = request.IsActive,
                CreatedBy = request.CreateBy,
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = request.LastModifiedBy,
                LastModifiedDate = DateTime.UtcNow
            };

            _db.Set<WorkDescription>().Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return new WorkDescriptionDto(entity.ID, entity.WorkDescriptionName, entity.IsActive);
        }
        catch (AppException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to create work description '{request.WorkDescriptionName}'. Database error: {ex.GetBaseException().Message}", "WORK_DESCRIPTION_CREATE_DATABASE_ERROR");
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to create work description '{request.WorkDescriptionName}'. Error: {ex.Message}", 500, "WORK_DESCRIPTION_CREATE_ERROR");
        }
    }
}

internal sealed class UpdateWorkDescriptionCommandHandler : IRequestHandler<UpdateWorkDescriptionCommand, WorkDescriptionDto?>
{
    private readonly IPMDbContext _db;

    public UpdateWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto?> Handle(UpdateWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkDescriptionName))
                throw new BadRequestException("Work description name is required.", "WORK_DESCRIPTION_NAME_REQUIRED");

            var entity = await _db.Set<WorkDescription>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
            if (entity is null) return null;

            var isDuplicate = await _db.Set<WorkDescription>().AsNoTracking()
                .Where(d => d.ID != request.Id)
                .AnyAsync(d => d.WorkDescriptionName.ToLower() == request.WorkDescriptionName.ToLower(), cancellationToken);
            if (isDuplicate)
                throw new BadRequestException($"A work description with name '{request.WorkDescriptionName}' already exists.", "WORK_DESCRIPTION_DUPLICATE");

            entity.WorkDescriptionName = request.WorkDescriptionName;
            entity.IsActive = request.IsActive;
            entity.LastModifiedBy = request.LastModifiedBy;
            entity.LastModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            return new WorkDescriptionDto(entity.ID, entity.WorkDescriptionName, entity.IsActive);
        }
        catch (AppException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to update work description with ID {request.Id}. Database error: {ex.GetBaseException().Message}", "WORK_DESCRIPTION_UPDATE_DATABASE_ERROR");
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to update work description with ID {request.Id}. Error: {ex.Message}", 500, "WORK_DESCRIPTION_UPDATE_ERROR");
        }
    }
}

internal sealed class DeleteWorkDescriptionCommandHandler : IRequestHandler<DeleteWorkDescriptionCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        var actionHistory = request.addTransactionActionHistoryDTO;

        try
        {
            var entity = await _db.Set<WorkDescription>()
                .FirstOrDefaultAsync(d => d.ID == actionHistory.ProgramRowId, cancellationToken);
            if (entity is null) return false;

            entity.IsActive = actionHistory.Actions == Actions.Activated;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to update work description status with ID {actionHistory.ProgramRowId}. Database error: {ex.GetBaseException().Message}", "WORK_DESCRIPTION_DELETE_DATABASE_ERROR");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to update work description status with ID {actionHistory.ProgramRowId}. Error: {ex.Message}", 500, "WORK_DESCRIPTION_DELETE_ERROR");
        }
    }
}

internal sealed class GetAllWorkDescriptionsQueryHandler : IRequestHandler<GetAllWorkDescriptionsQuery, IReadOnlyCollection<WorkDescriptionDto>>
{
    private readonly IPMDbContext _db;

    public GetAllWorkDescriptionsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<WorkDescriptionDto>> Handle(GetAllWorkDescriptionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _db.Set<WorkDescription>().AsNoTracking()
                .Where(d => d.IsActive)
                .Select(d => new WorkDescriptionDto(d.ID, d.WorkDescriptionName, d.IsActive))
                .ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new DbException($"Failed to retrieve work descriptions. Database error: {ex.GetBaseException().Message}", "WORK_DESCRIPTION_GET_ALL_DATABASE_ERROR");
        }
    }
}

internal sealed class GetWorkDescriptionByIdQueryHandler : IRequestHandler<GetWorkDescriptionByIdQuery, WorkDescriptionDto?>
{
    private readonly IPMDbContext _db;

    public GetWorkDescriptionByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto?> Handle(GetWorkDescriptionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _db.Set<WorkDescription>().AsNoTracking()
                .FirstOrDefaultAsync(d => d.ID == request.Id && d.IsActive, cancellationToken);
            return dto is null ? null : new WorkDescriptionDto(dto.ID, dto.WorkDescriptionName, dto.IsActive);
        }
        catch (Exception ex)
        {
            throw new DbException($"Failed to retrieve work description with ID {request.Id}. Database error: {ex.GetBaseException().Message}", "WORK_DESCRIPTION_GET_BY_ID_DATABASE_ERROR");
        }
    }
}


internal sealed class GetWorkDescriptionListQueryHandler : IRequestHandler<GetWorkDescriptionList, PagedResult<WorkDescriptionListItemDto>>
{
    private readonly IPMDbContext _db;
    public GetWorkDescriptionListQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PagedResult<WorkDescriptionListItemDto>> Handle(GetWorkDescriptionList request, CancellationToken cancellationToken)
    {
        try
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 200);
            var pageIndex = page - 1;

            var dbContext = _db as DbContext;
            if (dbContext is null)
                throw new DbException("Unable to retrieve work description list because the configured PM database context is invalid.", "WORK_DESCRIPTION_LIST_CONTEXT_ERROR");

            var items = new List<WorkDescriptionListItemDto>();
            long totalCount = 0;
            var connString = dbContext.Database.GetDbConnection().ConnectionString;

            using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            using (var cmd = new NpgsqlCommand(
    """
    SELECT cnt
    FROM "PAndM".uspgetworkdescriptioncount(
        CAST(@p_filtercolumn AS varchar),
        CAST(@p_filtervalue AS varchar),
        @p_pageindex,
        @p_pagesize,
        CAST(@p_sortcolumn AS varchar),
        CAST(@p_isactive AS varchar)
    )
    """, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 10;

                cmd.Parameters.AddWithValue(
                    "@p_filtercolumn",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.FilterColumn)
                        ? (object)DBNull.Value
                        : request.FilterColumn);

                cmd.Parameters.AddWithValue(
                    "@p_filtervalue",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.FilterValue)
                        ? (object)DBNull.Value
                        : request.FilterValue);

                cmd.Parameters.AddWithValue(
                    "@p_pageindex", NpgsqlDbType.Integer, pageIndex);

                cmd.Parameters.AddWithValue(
                    "@p_pagesize", NpgsqlDbType.Integer, pageSize);

                cmd.Parameters.AddWithValue(
                    "@p_sortcolumn",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.SortColumn)
                        ? (object)DBNull.Value
                        : request.SortColumn);

                cmd.Parameters.AddWithValue(
                    "@p_isactive",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.IsActive)
                        ? "all"
                        : request.IsActive);

                var countResult = await cmd.ExecuteScalarAsync(cancellationToken);

                totalCount = countResult != null && countResult != DBNull.Value
                    ? Convert.ToInt64(countResult)
                    : 0;
            }


            using (var cmd2 = new NpgsqlCommand(
                """
    SELECT *
    FROM "PAndM".uspgetworkdescription(
        CAST(@p_filtercolumn AS varchar),
        CAST(@p_filtervalue AS varchar),
        @p_pageindex,
        @p_pagesize,
        CAST(@p_sortcolumn AS varchar),
        CAST(@p_isactive AS varchar)
    )
    """, conn))
            {
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandTimeout = 30;

                cmd2.Parameters.AddWithValue(
                    "@p_filtercolumn",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.FilterColumn)
                        ? (object)DBNull.Value
                        : request.FilterColumn);

                cmd2.Parameters.AddWithValue(
                    "@p_filtervalue",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.FilterValue)
                        ? (object)DBNull.Value
                        : request.FilterValue);

                cmd2.Parameters.AddWithValue(
                    "@p_pageindex", NpgsqlDbType.Integer, pageIndex);

                cmd2.Parameters.AddWithValue(
                    "@p_pagesize", NpgsqlDbType.Integer, pageSize);

                cmd2.Parameters.AddWithValue(
                    "@p_sortcolumn",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.SortColumn)
                        ? (object)DBNull.Value
                        : request.SortColumn);

                cmd2.Parameters.AddWithValue(
                    "@p_isactive",
                    NpgsqlDbType.Text,
                    string.IsNullOrWhiteSpace(request.IsActive)
                        ? "all"
                        : request.IsActive);

                using var reader = await cmd2.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new WorkDescriptionListItemDto(
                        ID: reader.GetInt32(reader.GetOrdinal("ID")),
                        WorkDescriptionName: reader.GetString(
                            reader.GetOrdinal("WorkDescriptionName")),
                        IsActive: reader.GetBoolean(
                            reader.GetOrdinal("IsActive")),
                        UniqueID: reader.GetGuid(
                            reader.GetOrdinal("UniqueID")),
                        CreatedDate: reader.GetDateTime(
                            reader.GetOrdinal("CreatedDate")),
                        CreatedBy: reader.GetInt32(
                            reader.GetOrdinal("CreatedBy")),
                        LastModifiedDate: reader.GetDateTime(
                            reader.GetOrdinal("LastModifiedDate")),
                        LastModifiedBy: reader.GetInt32(
                            reader.GetOrdinal("LastModifiedBy"))
                    ));
                }
            }

            return new PagedResult<WorkDescriptionListItemDto>(
                items, page, pageSize, (int)totalCount);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (PostgresException ex)
        {
            var detail = string.IsNullOrWhiteSpace(ex.Detail) ? string.Empty : $" Detail: {ex.Detail}";
            var hint = string.IsNullOrWhiteSpace(ex.Hint) ? string.Empty : $" Hint: {ex.Hint}";
            throw new DbException($"Failed to retrieve the work description list from the database. PostgreSQL error {ex.SqlState}: {ex.MessageText}.{detail}{hint}", "WORK_DESCRIPTION_LIST_DATABASE_ERROR");
        }
        catch (DbException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to retrieve the work description list. Error: {ex.Message}", 500, "WORK_DESCRIPTION_LIST_ERROR");
        }
    }
}