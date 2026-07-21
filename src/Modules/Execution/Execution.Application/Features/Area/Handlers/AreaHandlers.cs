using Himapp.Execution.Application.Features.Area.Models;
using Himapp.Execution.Application.Features.Area.Commands;
using Himapp.Execution.Application.Features.Area.Queries;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Area.Handlers;

internal sealed class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, IReadOnlyCollection<AreaModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllAreasQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<AreaModel>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
    {
        return await _db.Areas
            .AsNoTracking()
            .Select(a => new AreaModel(a.Id, a.UniqueId, a.ProjectId, a.Name, a.Code, a.IsActive, a.CreatedBy, a.CreatedDate, a.LastModifiedBy, a.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetAreaByIdQueryHandler : IRequestHandler<GetAreaByIdQuery, AreaModel?>
{
    private readonly ExecutionDbContext _db;
    public GetAreaByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<AreaModel?> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _db.Areas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (a is null) return null;
        return new AreaModel(a.Id, a.UniqueId, a.ProjectId, a.Name, a.Code, a.IsActive, a.CreatedBy, a.CreatedDate, a.LastModifiedBy, a.LastModifiedDate);
    }
}

internal sealed class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, AreaModel>
{
    private readonly ExecutionDbContext _db;
    public CreateAreaCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<AreaModel> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = new Himapp.Execution.Domain.Entities.Area
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.Request.ProjectId,
            Name = request.Request.Name,
            Code = request.Request.Code,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Areas.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new AreaModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, AreaModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateAreaCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<AreaModel?> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Areas.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Request.Name;
        entity.Code = request.Request.Code;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new AreaModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteAreaCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Areas.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

