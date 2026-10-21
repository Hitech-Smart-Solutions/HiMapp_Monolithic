using Himapp.PM.Application.Features.SpareParts.Commands;
using Himapp.PM.Application.Features.SpareParts.Queries;
using Himapp.PM.Application.Features.SpareParts.Models;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.SpareParts.Handlers;

internal sealed class SparePartCatalogueHandlers :
    IRequestHandler<CreateSparePartCatalogueCommand, SparePartCatalogueDto>,
    IRequestHandler<UpdateSparePartCatalogueCommand, SparePartCatalogueDto?>,
    IRequestHandler<DeleteSparePartCatalogueCommand, bool>,
    IRequestHandler<GetAllSparePartCataloguesQuery, List<object>>,
    IRequestHandler<GetSparePartCatalogueByIdQuery, SparePartCatalogueDto?>
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
        var entity = await _db.Set<SparePartCatalogue>().FirstOrDefaultAsync(s => s.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
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
}
