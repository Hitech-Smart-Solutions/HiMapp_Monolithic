using Himapp.PM.Application.Features.SpecificationMaster.Commands;
using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.SpecificationMaster.Models;
using Himapp.PM.Application.Features.SpecificationMaster.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SpecificationMasterEntity = Himapp.PM.Domain.Entities.SpecificationMaster;

namespace Himapp.PM.Application.Features.SpecificationMaster.Handlers;

internal sealed class CreateSpecificationMasterCommandHandler : IRequestHandler<CreateSpecificationMasterCommand, SpecificationMasterDto>
{
    private readonly IPMDbContext _db;

    public CreateSpecificationMasterCommandHandler(IPMDbContext db) => _db = db;

    public async Task<SpecificationMasterDto> Handle(CreateSpecificationMasterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<SpecificationMasterEntity>().AsNoTracking()
            .AnyAsync(p => p.SpecificationName.ToLower() == request.SpecificationName.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"A specification with name '{request.SpecificationName}' already exists.");

        var entity = new SpecificationMasterEntity
        {
            UniqueID = Guid.NewGuid(),
            Type = (int)request.Type,
            SpecificationName = request.SpecificationName,
            IsActive = request.IsActive,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<SpecificationMasterEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}

internal sealed class UpdateSpecificationMasterCommandHandler : IRequestHandler<UpdateSpecificationMasterCommand, SpecificationMasterDto?>
{
    private readonly IPMDbContext _db;

    public UpdateSpecificationMasterCommandHandler(IPMDbContext db) => _db = db;

    public async Task<SpecificationMasterDto?> Handle(UpdateSpecificationMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<SpecificationMasterEntity>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        var duplicate = await _db.Set<SpecificationMasterEntity>().AsNoTracking()
            .Where(p => p.ID != request.Id)
            .AnyAsync(p => p.SpecificationName.ToLower() == request.SpecificationName.ToLower(), cancellationToken);
        if (duplicate)
            throw new InvalidOperationException($"A specification with name '{request.SpecificationName}' already exists.");

        entity.Type = (int)request.Type;
        entity.SpecificationName = request.SpecificationName;
        entity.IsActive = request.IsActive;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}

internal sealed class DeleteSpecificationMasterCommandHandler : IRequestHandler<DeleteSpecificationMasterCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteSpecificationMasterCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteSpecificationMasterCommand request, CancellationToken cancellationToken)
    {
        var actionHistory = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<SpecificationMasterEntity>()
            .FirstOrDefaultAsync(p => p.ID == actionHistory.ProgramRowId, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = actionHistory.Actions == Actions.Activated;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllSpecificationMastersQueryHandler : IRequestHandler<GetAllSpecificationMastersQuery, IReadOnlyCollection<SpecificationMasterDto>>
{
    private readonly IPMDbContext _db;

    public GetAllSpecificationMastersQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<SpecificationMasterDto>> Handle(GetAllSpecificationMastersQuery request, CancellationToken cancellationToken) =>
        await _db.Set<SpecificationMasterEntity>().AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new SpecificationMasterDto(
                p.ID,
                p.SpecificationName,
                p.IsActive,
                (SpecificationType)p.Type))
            .ToArrayAsync(cancellationToken);
}

internal sealed class GetSpecificationMasterByIdQueryHandler : IRequestHandler<GetSpecificationMasterByIdQuery, SpecificationMasterDto?>
{
    private readonly IPMDbContext _db;

    public GetSpecificationMasterByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<SpecificationMasterDto?> Handle(GetSpecificationMasterByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<SpecificationMasterEntity>().AsNoTracking()
            .FirstOrDefaultAsync(p => p.ID == request.Id && p.IsActive, cancellationToken);
        return entity?.ToDto();
    }
}

internal sealed class GetSpecificationMasterListHandler : IRequestHandler<GetSpecificationMasterList, PagedResult<SpecificationMasterListItemDto>>
{
    private readonly IPMDbContext _db;

    public GetSpecificationMasterListHandler(IPMDbContext db) => _db = db;

    public Task<PagedResult<SpecificationMasterListItemDto>> Handle(GetSpecificationMasterList request, CancellationToken cancellationToken) =>
        PagedStoredProcedureQueryExecutor.ExecuteAsync(_db, "uspgetspecificationmaster", "uspgetspecificationmastercount", request.Page, request.PageSize, request.FilterColumn, request.FilterValue, request.SortColumn, request.IsActive,
            reader => new SpecificationMasterListItemDto(
                reader.GetInt32(reader.GetOrdinal("ID")), reader.GetString(reader.GetOrdinal("SpecificationName")), reader.GetBoolean(reader.GetOrdinal("IsActive")),
                (SpecificationType)reader.GetInt32(reader.GetOrdinal("Type")), reader.GetGuid(reader.GetOrdinal("UniqueID")), reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                reader.GetInt32(reader.GetOrdinal("CreatedBy")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), reader.GetInt32(reader.GetOrdinal("LastModifiedBy"))), cancellationToken);
}

file static class SpecificationMasterMappings
{
    public static SpecificationMasterDto ToDto(this SpecificationMasterEntity entity) =>
        new(entity.ID, entity.SpecificationName, entity.IsActive, (SpecificationType)entity.Type);
}
