using Himapp.Execution.Application.Features.RateMaster.Models;
using Himapp.Execution.Application.Features.RateMaster.Commands;
using Himapp.Execution.Application.Features.RateMaster.Queries;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.RateMaster.Handlers;

internal sealed class CreateRateMasterCommandHandler : IRequestHandler<CreateRateMasterCommand, RateMasterModel>
{
    private readonly ExecutionDbContext _db;
    public CreateRateMasterCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<RateMasterModel> Handle(CreateRateMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = new Himapp.Execution.Domain.Entities.RateMaster
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = request.Request.ProjectId,
            ActivityID = request.Request.ActivityId,
            Rate = request.Request.Rate,
            UOMID = 0,
            EffectiveFrom = request.Request.EffectiveFrom,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.RateMasters.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new RateMasterModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.Rate, 0, entity.EffectiveFrom, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateRateMasterCommandHandler : IRequestHandler<UpdateRateMasterCommand, RateMasterModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateRateMasterCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<RateMasterModel?> Handle(UpdateRateMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.RateMasters.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectID = request.Request.ProjectId;
        entity.ActivityID = request.Request.ActivityId;
        entity.Rate = request.Request.Rate;
        entity.UOMID = 0;
        entity.EffectiveFrom = request.Request.EffectiveFrom;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new RateMasterModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.Rate, 0, entity.EffectiveFrom, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteRateMasterCommandHandler : IRequestHandler<DeleteRateMasterCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteRateMasterCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteRateMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.RateMasters.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllRateMastersQueryHandler : IRequestHandler<GetAllRateMastersQuery, IReadOnlyCollection<RateMasterModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllRateMastersQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<RateMasterModel>> Handle(GetAllRateMastersQuery request, CancellationToken cancellationToken)
    {
        return await _db.RateMasters
            .AsNoTracking()
            .Select(r => new RateMasterModel(r.ID, r.UniqueID, r.ProjectID, r.ActivityID, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetRateMasterByIdQueryHandler : IRequestHandler<GetRateMasterByIdQuery, RateMasterModel?>
{
    private readonly ExecutionDbContext _db;
    public GetRateMasterByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<RateMasterModel?> Handle(GetRateMasterByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _db.RateMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (r is null) return null;
        return new RateMasterModel(r.ID, r.UniqueID, r.ProjectID, r.ActivityID, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate);
    }
}

