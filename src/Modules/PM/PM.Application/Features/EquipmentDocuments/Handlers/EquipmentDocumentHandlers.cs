using Himapp.Api.src.Shared.Exceptions;
using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentDocuments.Commands;
using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using Himapp.PM.Application.Features.EquipmentDocuments.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Handlers;

internal sealed class CreateEquipmentDocumentCommandHandler : IRequestHandler<CreateEquipmentDocumentCommand, EquipmentDocumentDto>
{
    private readonly IPMDbContext _db;

    public CreateEquipmentDocumentCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentDocumentDto> Handle(CreateEquipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ValidateRequest(request.DocumentTypeID, request.DocumentName, request.StartDate, request.EndDate);
            await EnsureDocumentTypeExists(request.DocumentTypeID, cancellationToken);

            var entity = new EquipmentDocument
            {
                UniqueID = Guid.NewGuid(),
                DocumentTypeID = request.DocumentTypeID,
                DocumentName = request.DocumentName.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive ?? true,
                CreatedBy = request.CreateBy,
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = request.LastModifiedBy,
                LastModifiedDate = DateTime.UtcNow
            };

            _db.Set<EquipmentDocument>().Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return ToDto(entity);
        }
        catch (AppException)
        {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to create equipment document '{request.DocumentName}'. Database error: {ex.GetBaseException().Message}", "EQUIPMENT_DOCUMENT_CREATE_DATABASE_ERROR");
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to create equipment document '{request.DocumentName}'. Error: {ex.Message}", 500, "EQUIPMENT_DOCUMENT_CREATE_ERROR");
        }
    }

    private async Task EnsureDocumentTypeExists(int documentTypeId, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<DocumentType>().AsNoTracking()
            .AnyAsync(d => d.ID == documentTypeId && d.IsActive == true, cancellationToken);
        if (!exists)
            throw new BadRequestException($"Active document type with ID {documentTypeId} was not found.", "EQUIPMENT_DOCUMENT_DOCUMENT_TYPE_NOT_FOUND");
    }

    private static void ValidateRequest(int documentTypeId, string documentName, DateTime startDate, DateTime endDate)
    {
        if (documentTypeId <= 0)
            throw new BadRequestException("Document type is required.", "EQUIPMENT_DOCUMENT_DOCUMENT_TYPE_REQUIRED");
        if (string.IsNullOrWhiteSpace(documentName))
            throw new BadRequestException("Document name is required.", "EQUIPMENT_DOCUMENT_NAME_REQUIRED");
        if (endDate < startDate)
            throw new BadRequestException("End date cannot be earlier than start date.", "EQUIPMENT_DOCUMENT_INVALID_DATE_RANGE");
    }

    internal static EquipmentDocumentDto ToDto(EquipmentDocument entity) =>
        new(entity.ID, entity.DocumentTypeID, entity.DocumentName, entity.StartDate, entity.EndDate, entity.IsActive);
}

internal sealed class UpdateEquipmentDocumentCommandHandler : IRequestHandler<UpdateEquipmentDocumentCommand, EquipmentDocumentDto>
{
    private readonly IPMDbContext _db;

    public UpdateEquipmentDocumentCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentDocumentDto> Handle(UpdateEquipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ValidateRequest(request.DocumentTypeID, request.DocumentName, request.StartDate, request.EndDate);

            var entity = await _db.Set<EquipmentDocument>()
                .FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Equipment document with ID {request.Id} was not found.", "EQUIPMENT_DOCUMENT_NOT_FOUND");

            await EnsureDocumentTypeExists(request.DocumentTypeID, cancellationToken);

            entity.DocumentTypeID = request.DocumentTypeID;
            entity.DocumentName = request.DocumentName.Trim();
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.IsActive = request.IsActive;
            entity.LastModifiedBy = request.LastModifiedBy;
            entity.LastModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            return CreateEquipmentDocumentCommandHandler.ToDto(entity);
        }
        catch (AppException)
        {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to update equipment document with ID {request.Id}. Database error: {ex.GetBaseException().Message}", "EQUIPMENT_DOCUMENT_UPDATE_DATABASE_ERROR");
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to update equipment document with ID {request.Id}. Error: {ex.Message}", 500, "EQUIPMENT_DOCUMENT_UPDATE_ERROR");
        }
    }

    private async Task EnsureDocumentTypeExists(int documentTypeId, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<DocumentType>().AsNoTracking()
            .AnyAsync(d => d.ID == documentTypeId && d.IsActive == true, cancellationToken);
        if (!exists)
            throw new BadRequestException($"Active document type with ID {documentTypeId} was not found.", "EQUIPMENT_DOCUMENT_DOCUMENT_TYPE_NOT_FOUND");
    }

    private static void ValidateRequest(int documentTypeId, string documentName, DateTime startDate, DateTime endDate)
    {
        if (documentTypeId <= 0)
            throw new BadRequestException("Document type is required.", "EQUIPMENT_DOCUMENT_DOCUMENT_TYPE_REQUIRED");
        if (string.IsNullOrWhiteSpace(documentName))
            throw new BadRequestException("Document name is required.", "EQUIPMENT_DOCUMENT_NAME_REQUIRED");
        if (endDate < startDate)
            throw new BadRequestException("End date cannot be earlier than start date.", "EQUIPMENT_DOCUMENT_INVALID_DATE_RANGE");
    }
}

internal sealed class DeleteEquipmentDocumentCommandHandler : IRequestHandler<DeleteEquipmentDocumentCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteEquipmentDocumentCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteEquipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _db.Set<EquipmentDocument>()
                .FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Equipment document with ID {request.Id} was not found.", "EQUIPMENT_DOCUMENT_NOT_FOUND");

            entity.IsActive = false;
            entity.LastModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (AppException)
        {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new DbException($"Failed to delete equipment document with ID {request.Id}. Database error: {ex.GetBaseException().Message}", "EQUIPMENT_DOCUMENT_DELETE_DATABASE_ERROR");
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to delete equipment document with ID {request.Id}. Error: {ex.Message}", 500, "EQUIPMENT_DOCUMENT_DELETE_ERROR");
        }
    }
}

internal sealed class GetAllEquipmentDocumentsQueryHandler : IRequestHandler<GetAllEquipmentDocumentsQuery, IReadOnlyCollection<EquipmentDocumentDto>>
{
    private readonly IPMDbContext _db;

    public GetAllEquipmentDocumentsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<EquipmentDocumentDto>> Handle(GetAllEquipmentDocumentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _db.Set<EquipmentDocument>().AsNoTracking()
                .Where(d => d.IsActive == true)
                .Select(d => new EquipmentDocumentDto(d.ID, d.DocumentTypeID, d.DocumentName, d.StartDate, d.EndDate, d.IsActive))
                .ToArrayAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DbException($"Failed to retrieve equipment documents. Database error: {ex.GetBaseException().Message}", "EQUIPMENT_DOCUMENT_GET_ALL_DATABASE_ERROR");
        }
    }
}

internal sealed class GetEquipmentDocumentByIdQueryHandler : IRequestHandler<GetEquipmentDocumentByIdQuery, EquipmentDocumentDto?>
{
    private readonly IPMDbContext _db;

    public GetEquipmentDocumentByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentDocumentDto?> Handle(GetEquipmentDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _db.Set<EquipmentDocument>().AsNoTracking().FirstOrDefaultAsync(d => d.ID == request.Id,cancellationToken);
            return entity is null ? null : CreateEquipmentDocumentCommandHandler.ToDto(entity);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DbException($"Failed to retrieve equipment document with ID {request.Id}. Database error: {ex.GetBaseException().Message}", "EQUIPMENT_DOCUMENT_GET_BY_ID_DATABASE_ERROR");
        }
    }
}

internal sealed class GetEquipmentDocumentListQueryHandler : IRequestHandler<GetEquipmentDocumentList, PagedResult<EquipmentDocumentListItemDto>>
{
    private readonly IPMDbContext _db;

    public GetEquipmentDocumentListQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PagedResult<EquipmentDocumentListItemDto>> Handle(GetEquipmentDocumentList request, CancellationToken cancellationToken)
    {
        try
        {
            return await PagedStoredProcedureQueryExecutor.ExecuteAsync(
                _db,
                "uspgetequipmentdocuments",
                "uspgetequipmentdocumentscount",
                request.Page,
                request.PageSize,
                request.FilterColumn,
                request.FilterValue,
                request.SortColumn,
                request.IsActive,
                reader => new EquipmentDocumentListItemDto(
                    ID: reader.GetInt32(reader.GetOrdinal("ID")),
                    DocumentTypeID: reader.GetInt32(reader.GetOrdinal("DocumentTypeID")),
                    DocumentName: reader.GetString(reader.GetOrdinal("DocumentName")),
                    DocumentTypeName: reader.GetString(reader.GetOrdinal("DocumentTypeName")),
                    StartDate: reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate: reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    IsActive: reader.IsDBNull(reader.GetOrdinal("IsActive"))
                        ? null
                        : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    UniqueID: reader.GetGuid(reader.GetOrdinal("UniqueID")),
                    CreatedDate: reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CreatedBy: reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                    LastModifiedDate: reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")),
                    LastModifiedBy: reader.GetInt32(reader.GetOrdinal("LastModifiedBy"))),
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (PostgresException ex)
        {
            var detail = string.IsNullOrWhiteSpace(ex.Detail) ? string.Empty : $" Detail: {ex.Detail}";
            var hint = string.IsNullOrWhiteSpace(ex.Hint) ? string.Empty : $" Hint: {ex.Hint}";
            throw new DbException($"Failed to retrieve the equipment document list from the database. PostgreSQL error {ex.SqlState}: {ex.MessageText}.{detail}{hint}", "EQUIPMENT_DOCUMENT_LIST_DATABASE_ERROR");
        }
        catch (DbException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AppException($"Failed to retrieve the equipment document list. Error: {ex.Message}", 500, "EQUIPMENT_DOCUMENT_LIST_ERROR");
        }
    }
}
