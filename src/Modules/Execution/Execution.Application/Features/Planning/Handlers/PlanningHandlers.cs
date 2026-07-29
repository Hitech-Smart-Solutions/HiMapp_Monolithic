using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Planning.Handlers;

internal sealed class PlanningHandlers :
    IRequestHandler<GetAllPlanningsQuery, IReadOnlyCollection<PlanningModel>>,
    IRequestHandler<GetPlanningByIdQuery, PlanningModel?>,
    IRequestHandler<CreatePlanningCommand, PlanningModel>,
    IRequestHandler<UpdatePlanningCommand, PlanningModel?>,
    IRequestHandler<DeletePlanningCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public PlanningHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<PlanningModel>> Handle(GetAllPlanningsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Select(p => new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.PlanType, p.StartDate, p.EndDate, p.Remarks, p.Status, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PlanningModel?> Handle(GetPlanningByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;
        return new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.PlanType, p.StartDate, p.EndDate, p.Remarks, p.Status, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }

    public async Task<PlanningModel> Handle(CreatePlanningCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.Planning
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            PlanType = r.PlanType,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Remarks = r.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Himapp.Execution.Domain.Entities.Planning>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<PlanningModel?> Handle(UpdatePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> Handle(DeletePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

