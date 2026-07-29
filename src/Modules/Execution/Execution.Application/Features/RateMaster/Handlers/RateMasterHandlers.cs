using Himapp.Execution.Application.Features.RateMaster.Models;
using Himapp.Execution.Application.Features.RateMaster.Commands;
using Himapp.Execution.Application.Features.RateMaster.Queries;
using Himapp.Execution.Application.Features.RateMaster.Models;
using Himapp.Execution.Application.Features.RateMaster.Commands;
using Himapp.Execution.Application.Features.RateMaster.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.RateMaster.Handlers;

internal sealed class RateMasterHandlers :
    IRequestHandler<CreateRateMasterCommand, RateMasterModel>,
    IRequestHandler<UpdateRateMasterCommand, RateMasterModel?>,
    IRequestHandler<DeleteRateMasterCommand, bool>,
    IRequestHandler<GetAllRateMastersQuery, IReadOnlyCollection<RateMasterModel>>,
    IRequestHandler<GetRateMasterByIdQuery, RateMasterModel?>
{
    private readonly IExecutionDbContext _db;
    public RateMasterHandlers(IExecutionDbContext db) => _db = db;

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

        _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new RateMasterModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.Rate, 0, entity.EffectiveFrom, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<RateMasterModel?> Handle(UpdateRateMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
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

    public async Task<bool> Handle(DeleteRateMasterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<RateMasterModel>> Handle(GetAllRateMastersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>()
            .AsNoTracking()
            .Select(r => new RateMasterModel(r.ID, r.UniqueID, r.ProjectID, r.ActivityID, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<RateMasterModel?> Handle(GetRateMasterByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (r is null) return null;
        return new RateMasterModel(r.ID, r.UniqueID, r.ProjectID, r.ActivityID, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate);
    }
}

