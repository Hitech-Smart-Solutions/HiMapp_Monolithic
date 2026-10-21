using Himapp.PM.Application.Features.WorkDescriptions.Commands;
using Himapp.PM.Application.Features.WorkDescriptions.Models;
using Himapp.PM.Application.Features.WorkDescriptions.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.WorkDescriptions.Handlers;

internal sealed class CreateWorkDescriptionCommandHandler : IRequestHandler<CreateWorkDescriptionCommand, WorkDescriptionDto>
{
    private readonly IPMDbContext _db;

    public CreateWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto> Handle(CreateWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<WorkDescription>().AsNoTracking()
            .AnyAsync(d => d.WorkDescriptionName.ToLower() == request.WorkDescriptionName.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"A work description with name '{request.WorkDescriptionName}' already exists.");

        var entity = new WorkDescription
        {
            UniqueID = Guid.NewGuid(),
            WorkDescriptionName = request.WorkDescriptionName,
            IsActive = request.IsActive,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<WorkDescription>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new WorkDescriptionDto(entity.ID, entity.WorkDescriptionName, entity.IsActive);
    }
}

internal sealed class UpdateWorkDescriptionCommandHandler : IRequestHandler<UpdateWorkDescriptionCommand, WorkDescriptionDto?>
{
    private readonly IPMDbContext _db;

    public UpdateWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto?> Handle(UpdateWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<WorkDescription>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        var isDuplicate = await _db.Set<WorkDescription>().AsNoTracking()
            .Where(d => d.ID != request.Id)
            .AnyAsync(d => d.WorkDescriptionName.ToLower() == request.WorkDescriptionName.ToLower(), cancellationToken);
        if (isDuplicate)
            throw new InvalidOperationException($"A work description with name '{request.WorkDescriptionName}' already exists.");

        entity.WorkDescriptionName = request.WorkDescriptionName;
        entity.IsActive = request.IsActive;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return new WorkDescriptionDto(entity.ID, entity.WorkDescriptionName, entity.IsActive);
    }
}

internal sealed class DeleteWorkDescriptionCommandHandler : IRequestHandler<DeleteWorkDescriptionCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteWorkDescriptionCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteWorkDescriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<WorkDescription>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllWorkDescriptionsQueryHandler : IRequestHandler<GetAllWorkDescriptionsQuery, IReadOnlyCollection<WorkDescriptionDto>>
{
    private readonly IPMDbContext _db;

    public GetAllWorkDescriptionsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<WorkDescriptionDto>> Handle(GetAllWorkDescriptionsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<WorkDescription>().AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new WorkDescriptionDto(d.ID, d.WorkDescriptionName, d.IsActive))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetWorkDescriptionByIdQueryHandler : IRequestHandler<GetWorkDescriptionByIdQuery, WorkDescriptionDto?>
{
    private readonly IPMDbContext _db;

    public GetWorkDescriptionByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<WorkDescriptionDto?> Handle(GetWorkDescriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.Set<WorkDescription>().AsNoTracking()
            .FirstOrDefaultAsync(d => d.ID == request.Id && d.IsActive, cancellationToken);
        return dto is null ? null : new WorkDescriptionDto(dto.ID, dto.WorkDescriptionName, dto.IsActive);
    }
}
