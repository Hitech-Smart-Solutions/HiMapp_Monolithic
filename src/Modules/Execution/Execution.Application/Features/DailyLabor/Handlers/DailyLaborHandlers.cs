using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class GetAllDailyLaborsQueryHandler : IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllDailyLaborsQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken)
    {
        // Return header-only projection for performance (details omitted)
        return await _db.DailyLabors
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyLaborModel(
                d.ID,
                d.UniqueID,
                d.CompanyID,
                d.ProjectID,
                d.DLRDate,
                d.Remarks,
                d.StateID,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyLaborDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetDailyLaborByIdQueryHandler : IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>
{
    private readonly ExecutionDbContext _db;
    public GetDailyLaborByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyLabors
            .AsNoTracking()
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(
            dd.ID,
            dd.UniqueID,
            dd.ContractorID,
            dd.CategoryID,
            dd.Skilled,
            dd.UnSkilled,
            dd.Remarks,
            dd.Mat,
            dd.ContractorName,
            dd.ProductivityID)).ToArray()
            ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(
            entity.ID,
            entity.UniqueID,
            entity.CompanyID,
            entity.ProjectID,
            entity.DLRDate,
            entity.Remarks,
            entity.StateID,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedDate,
            entity.LastModifiedBy,
            entity.LastModifiedDate,
            details);
    }
}

internal sealed class CreateDailyLaborCommandHandler : IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>
{
    private readonly ExecutionDbContext _db;
    public CreateDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel> Handle(CreateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.DailyLabor
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            DLRDate = r.ReportDate,
            Remarks = r.Remarks,
            StateID = (short?)r.Status,
            IsActive = true,
            CreatedBy = 0,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = 0,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyLaborDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    CategoryID = d.CategoryId,
                    Skilled = d.Skilled,
                    UnSkilled = d.UnSkilled,
                    Remarks = d.Remarks,
                    Mat = d.Mat,
                    ContractorName = d.ContractorName,
                    ProductivityID = d.ProductivityId,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        _db.DailyLabors.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ProductivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
}

internal sealed class UpdateDailyLaborCommandHandler : IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel?> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyLabors
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.DLRDate = r.ReportDate;
        entity.Remarks = r.Remarks;
        entity.StateID = (short?)r.Status;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details (physically) and add new ones
        if (entity.DailyLaborDetail != null && entity.DailyLaborDetail.Any())
        {
            _db.DailyLaborDetails.RemoveRange(entity.DailyLaborDetail);
            entity.DailyLaborDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyLaborDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    CategoryID = d.CategoryId,
                    Skilled = d.Skilled,
                    UnSkilled = d.UnSkilled,
                    Remarks = d.Remarks,
                    Mat = d.Mat,
                    ContractorName = d.ContractorName,
                    ProductivityID = d.ProductivityId,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ProductivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
}

internal sealed class DeleteDailyLaborCommandHandler : IRequestHandler<DeleteDailyLaborCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyLabors.Include(d => d.DailyLaborDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyLaborDetail != null)
        {
            foreach (var dd in entity.DailyLaborDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

