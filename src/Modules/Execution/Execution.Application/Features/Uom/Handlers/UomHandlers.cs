using Himapp.Execution.Application.Features.Uom.Models;
using Himapp.Execution.Application.Features.Uom.Commands;
using Himapp.Execution.Application.Features.Uom.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Uom.Handlers;

internal sealed class GetAllUomsQueryHandler : IRequestHandler<GetAllUomsQuery, IReadOnlyCollection<UomModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllUomsQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<UomModel>> Handle(GetAllUomsQuery request, CancellationToken cancellationToken)
    {
        return await _db.UOMs
            .AsNoTracking()
            .Select(u => new UomModel(u.Id, u.UniqueId, u.CompanyId, u.Name, u.Code, u.IsActive, u.CreatedBy, u.CreatedDate, u.LastModifiedBy, u.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetUomByIdQueryHandler : IRequestHandler<GetUomByIdQuery, UomModel?>
{
    private readonly ExecutionDbContext _db;
    public GetUomByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<UomModel?> Handle(GetUomByIdQuery request, CancellationToken cancellationToken)
    {
        var u = await _db.UOMs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (u is null) return null;
        return new UomModel(u.Id, u.UniqueId, u.CompanyId, u.Name, u.Code, u.IsActive, u.CreatedBy, u.CreatedDate, u.LastModifiedBy, u.LastModifiedDate);
    }
}

internal sealed class CreateUomCommandHandler : IRequestHandler<CreateUomCommand, UomModel>
{
    private readonly ExecutionDbContext _db;
    public CreateUomCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<UomModel> Handle(CreateUomCommand request, CancellationToken cancellationToken)
    {
        var entity = new UOM
        {
            UniqueId = Guid.NewGuid(),
            CompanyId = request.Request.CompanyId,
            Name = request.Request.Name,
            Code = request.Request.Code,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.UOMs.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new UomModel(entity.Id, entity.UniqueId, entity.CompanyId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateUomCommandHandler : IRequestHandler<UpdateUomCommand, UomModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateUomCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<UomModel?> Handle(UpdateUomCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.UOMs.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Request.Name;
        entity.Code = request.Request.Code;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new UomModel(entity.Id, entity.UniqueId, entity.CompanyId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteUomCommandHandler : IRequestHandler<DeleteUomCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteUomCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteUomCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.UOMs.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

