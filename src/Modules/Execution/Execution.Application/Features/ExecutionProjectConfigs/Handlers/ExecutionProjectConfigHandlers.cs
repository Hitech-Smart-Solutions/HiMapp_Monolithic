using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Commands;
using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;
using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Queries;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.ExecutionProjectConfigs.Handlers;

internal sealed class ExecutionProjectConfigHandlers :
    IRequestHandler<CreateExecutionProjectConfigCommand, ExecutionProjectConfigModel>,
    IRequestHandler<UpdateExecutionProjectConfigCommand, ExecutionProjectConfigModel?>,
    IRequestHandler<GetExecutionProjectConfigByProjectIdQuery, ExecutionProjectConfigModel?>
{
    private readonly IExecutionDbContext _db;

    public ExecutionProjectConfigHandlers(IExecutionDbContext db) => _db = db;

    public async Task<ExecutionProjectConfigModel> Handle(
        CreateExecutionProjectConfigCommand request,
        CancellationToken cancellationToken)
    {
        var config = request.Request;
        var now = DateTimeOffset.UtcNow;
        var entity = new ExecutionProjectConfig
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = config.ProjectId,
            MaxHours = config.MaxHours,
            IsActive = true,
            CreatedBy = config.CreatedBy,
            CreatedDate = now,
            LastModifiedBy = config.CreatedBy,
            LastModifiedDate = now
        };

        _db.Set<ExecutionProjectConfig>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<ExecutionProjectConfigModel?> Handle(
    UpdateExecutionProjectConfigCommand request,
    CancellationToken cancellationToken)

    {
        var entity = await _db.Set<ExecutionProjectConfig>()
            .FirstOrDefaultAsync(config => config.ID == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var config = request.Request;
        entity.ProjectID = config.ProjectId;
        entity.MaxHours = config.MaxHours;
        entity.IsActive = config.IsActive;
        entity.LastModifiedBy = config.LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return ToModel(entity);
    }

    public async Task<ExecutionProjectConfigModel?> Handle(
        GetExecutionProjectConfigByProjectIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ExecutionProjectConfig>()
            .AsNoTracking()
            .FirstOrDefaultAsync(config => config.ProjectID == request.ProjectId, cancellationToken);

        return entity is null ? null : ToModel(entity);
    }

    private static ExecutionProjectConfigModel ToModel(ExecutionProjectConfig entity) => new(
        entity.ID,
        entity.UniqueID,
        entity.ProjectID,
        entity.MaxHours,
        entity.IsActive,
        entity.CreatedBy,
        entity.CreatedDate,
        entity.LastModifiedBy,
        entity.LastModifiedDate);
}
