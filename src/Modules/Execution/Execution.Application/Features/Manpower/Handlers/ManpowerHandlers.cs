using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class GetAllManpowersQueryHandler : IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllManpowersQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ManpowerModel>> Handle(GetAllManpowersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Manpowers
            .AsNoTracking()
            .Select(m => new ManpowerModel(m.Id, m.UniqueId, m.ProjectId, m.EntryDate, m.Shift, m.Remarks, m.Status, m.IsActive, m.CreatedBy, m.CreatedDate, m.LastModifiedBy, m.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetManpowerByIdQueryHandler : IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>
{
    private readonly ExecutionDbContext _db;
    public GetManpowerByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel?> Handle(GetManpowerByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _db.Manpowers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (m is null) return null;
        return new ManpowerModel(m.Id, m.UniqueId, m.ProjectId, m.EntryDate, m.Shift, m.Remarks, m.Status, m.IsActive, m.CreatedBy, m.CreatedDate, m.LastModifiedBy, m.LastModifiedDate);
    }
}

internal sealed class CreateManpowerCommandHandler : IRequestHandler<CreateManpowerCommand, ManpowerModel>
{
    private readonly ExecutionDbContext _db;
    public CreateManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel> Handle(CreateManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Himapp.Execution.Domain.Entities.Manpower
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.Request.ProjectId,
            EntryDate = request.Request.EntryDate,
            Shift = request.Request.Shift,
            Remarks = request.Request.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Manpowers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ManpowerModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.EntryDate, entity.Shift, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateManpowerCommandHandler : IRequestHandler<UpdateManpowerCommand, ManpowerModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel?> Handle(UpdateManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Manpowers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ManpowerModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.EntryDate, entity.Shift, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteManpowerCommandHandler : IRequestHandler<DeleteManpowerCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Manpowers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

