using Himapp.PM.Application.Features.DocumentTypes.Commands;
using Himapp.PM.Application.Features.DocumentTypes.Queries;
using Himapp.PM.Application.Features.DocumentTypes.Models;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Himapp.PM.Application;

namespace Himapp.PM.Application.Features.DocumentTypes.Handlers;

internal sealed class CreateDocumentTypeCommandHandler : IRequestHandler<CreateDocumentTypeCommand, DocumentTypeDto>
{
    private readonly IPMDbContext _db;
    public CreateDocumentTypeCommandHandler(IPMDbContext db) => _db = db;
    public async Task<DocumentTypeDto> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<DocumentType>().AsNoTracking().AnyAsync(d => d.DocumentTypeName.ToLower() == request.DocumentTypeName.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"A document type with name '{request.DocumentTypeName}' already exists.");

        var entity = new DocumentType
        {
            UniqueID = Guid.NewGuid(),
            DocumentTypeName = request.DocumentTypeName,
            IsActive = true,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow,
            Remarks = request.Remarks
        };

        _db.Set<DocumentType>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DocumentTypeDto(entity.ID, entity.DocumentTypeName);
    }
}

internal sealed class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, DocumentTypeDto?>
{
    private readonly IPMDbContext _db;
    public UpdateDocumentTypeCommandHandler(IPMDbContext db) => _db = db;
    public async Task<DocumentTypeDto?> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DocumentType>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        // check duplicate name
        var isDuplicate = await _db.Set<DocumentType>().AsNoTracking()
            .Where(d => d.ID != request.Id)
            .AnyAsync(d => d.DocumentTypeName.ToLower() == request.DocumentTypeName.ToLower(), cancellationToken);
        if (isDuplicate)
            throw new InvalidOperationException($"A document type with name '{request.DocumentTypeName}' already exists.");

        entity.DocumentTypeName = request.DocumentTypeName;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;
        entity.Remarks = request.Remarks;

        await _db.SaveChangesAsync(cancellationToken);
        return new DocumentTypeDto(entity.ID, entity.DocumentTypeName);
    }
}

internal sealed class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, bool>
{
    private readonly IPMDbContext _db;
    public DeleteDocumentTypeCommandHandler(IPMDbContext db) => _db = db;
    public async Task<bool> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DocumentType>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, IReadOnlyCollection<DocumentTypeDto>>
{
    private readonly IPMDbContext _db;
    public GetAllDocumentTypesQueryHandler(IPMDbContext db) => _db = db;
    public async Task<IReadOnlyCollection<DocumentTypeDto>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<DocumentType>().AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DocumentTypeDto(d.ID, d.DocumentTypeName))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeDto?>
{
    private readonly IPMDbContext _db;
    public GetDocumentTypeByIdQueryHandler(IPMDbContext db) => _db = db;
    public async Task<DocumentTypeDto?> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.Set<DocumentType>().AsNoTracking().FirstOrDefaultAsync(d => d.ID == request.Id && d.IsActive, cancellationToken);
        return dto is null ? null : new DocumentTypeDto(dto.ID, dto.DocumentTypeName);
    }
}
