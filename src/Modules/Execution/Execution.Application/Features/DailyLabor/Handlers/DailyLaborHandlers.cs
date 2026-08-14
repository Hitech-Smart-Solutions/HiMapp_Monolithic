using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using Himapp.Admin.Contracts.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class DailyLaborHandlers :
    IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>,
    IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>,
    IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>,
    IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel?>,
    IRequestHandler<DeleteDailyLaborCommand, bool>,
    IRequestHandler<DeleteDailyLaborActionCommand, bool>,
    IRequestHandler<GetConsolidatedDailyLaborQuery, IReadOnlyCollection<DailyLaborConsolidatedModel>>,
    IRequestHandler<GetDailyLaborByProjectID, PagedResult<DailyLaborModel>>
{
    private readonly IExecutionDbContext _db;
    private readonly IProjectDirectory _projectDirectory;
    private readonly Himapp.Execution.Contracts.References.IDlrCodeGenerator _codeGenerator;
    private readonly ICurrentUser _currentUser;
    public DailyLaborHandlers(IExecutionDbContext db, IProjectDirectory projectDirectory, Himapp.Execution.Contracts.References.IDlrCodeGenerator codeGenerator, ICurrentUser currentUser) => (_db, _projectDirectory, _codeGenerator, _currentUser) = (db, projectDirectory, codeGenerator, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken)
    {
        // Return header-only projection for performance (details omitted)
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyLaborModel(
                d.ID,
                d.UniqueID,
                d.DLRCode,
                d.CompanyID,
                d.ProjectID,
                d.DLRDate,
                d.Remarks,
                d.ProposedActionPlan,
                d.ConstraintsAndReasons,
                d.RemoveMenPower,
                d.StateID,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyLaborDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> Handle(DeleteDailyLaborActionCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<Himapp.Execution.Domain.Entities.DailyLaborDetail>()
            .Where(x => x.DailyLabourID == model.ProgramRowId)
            .ToListAsync(cancellationToken);

        foreach (var d in details)
        {
            d.IsActive = model.Actions == Actions.Activated ? true : false;
        }

        // Mark main entity active/inactive
        entity.IsActive = model.Actions == Actions.Activated ? true : false;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
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
            dd.ActivityID)).ToArray()
            ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(
            entity.ID,
            entity.UniqueID,
            entity.DLRCode,
            entity.CompanyID,
            entity.ProjectID,
            entity.DLRDate,
            entity.Remarks,
            entity.ProposedActionPlan,
            entity.ConstraintsAndReasons,
            entity.RemoveMenPower,
            entity.StateID,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedDate,
            entity.LastModifiedBy,
            entity.LastModifiedDate,
            details);
    }

    public async Task<DailyLaborModel> Handle(CreateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

        if (!Enum.IsDefined(typeof(DailyLaborState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Daily Labor status.");
        }

        var entity = new Himapp.Execution.Domain.Entities.DailyLabor
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ConstraintsAndReasons = r.ConstraintsAndReasons,
            ProposedActionPlan = r.ProposedActionPlan,
            RemoveMenPower = r.RemoveMenPower,
            CompanyID = r.CompanyID,
            DLRDate = DateTime.SpecifyKind(r.ReportDate, DateTimeKind.Utc),
            Remarks = r.Remarks,
            StateID = (short)r.Status,
            IsActive = true,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = userId,
            LastModifiedDate = DateTime.UtcNow
        };

        // Generate DLRCode using shared DLR code generator service
        var projectId = r.ProjectId;
        var generatedCode = await _codeGenerator.GenerateDLRCodeAsync(projectId, cancellationToken);
        entity.DLRCode = string.IsNullOrWhiteSpace(generatedCode) ? null : generatedCode;

        // Add details (if any)
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
                    ActivityID = d.ActivityId,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.DLRCode, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.ProposedActionPlan, entity.ConstraintsAndReasons, entity.RemoveMenPower, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Include(d => d.DailyLaborDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyLaborDetail != null)
        {
            foreach (var dd in entity.DailyLaborDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DailyLaborModel?> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var r = request.Request;

        if (!Enum.IsDefined(typeof(DailyLaborState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Daily Labor status.");
        }

        if (entity.StateID == (short)DailyLaborState.Submitted)
        {
            throw new InvalidOperationException(
                "Submitted Daily Labor cannot be modified.");
        }

        entity.ProjectID = r.ProjectId;
        entity.DLRDate = DateTime.SpecifyKind(r.ReportDate, DateTimeKind.Utc);
        entity.Remarks = r.Remarks;
        entity.RemoveMenPower = r.RemoveMenPower;
        entity.StateID = (short?)r.Status;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details (physically) and add new ones
        if (entity.DailyLaborDetail != null && entity.DailyLaborDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.DailyLaborDetail>().RemoveRange(entity.DailyLaborDetail);
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
                    ActivityID = d.ActivityId,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.DLRCode, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.ProposedActionPlan, entity.ConstraintsAndReasons, entity.RemoveMenPower, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<IReadOnlyCollection<DailyLaborConsolidatedModel>> Handle(GetConsolidatedDailyLaborQuery request, CancellationToken cancellationToken)
    {
        var result = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>()
            .AsNoTracking()
            .Where(m =>
                m.ProjectID == request.ProjectId &&
                m.EntryDate == request.Date &&
                m.IsActive)
            .SelectMany(m => m.ManpowerDetail!
                .Where(md => md.IsActive)
                .Select(md => new
                {
                    md.ContractorID,
                    md.ActivityID,
                    md.SkilledCount,
                    md.UnskilledCount,
                    md.OtherCount
                }))
            .GroupBy(x => new
            {
                x.ContractorID,
                x.ActivityID
            })
            .Select(g => new DailyLaborConsolidatedModel(
                g.Key.ContractorID,
                g.Key.ActivityID,
                g.Sum(x => x.SkilledCount),
                g.Sum(x => x.UnskilledCount),
                g.Sum(x => x.OtherCount),
                g.Sum(x =>
                    x.SkilledCount +
                    x.UnskilledCount +
                    x.OtherCount)
            ))
            .ToArrayAsync(cancellationToken);

        return result;
    }

    public async Task<PagedResult<DailyLaborModel>> Handle(GetDailyLaborByProjectID request, CancellationToken cancellationToken)
    {
        var p = request.SearchParamsProjectWise ?? new SearchParamsProjectWise();
        var query = _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().AsNoTracking().Where(x => x.ProjectID == p.ProjectID);
        if (bool.TryParse(p.IsActive, out var isActive)) query = query.Where(x => x.IsActive == isActive);
        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = p.PageSize > 0 ? p.PageSize : totalCount;
        var items = await query.OrderBy(x => x.ID).Skip(Math.Max(p.PageIndex, 0) * pageSize).Take(pageSize)
            .Select(x => new DailyLaborModel(x.ID, x.UniqueID, x.DLRCode, x.CompanyID, x.ProjectID, x.DLRDate, x.Remarks, x.ProposedActionPlan, x.ConstraintsAndReasons, x.RemoveMenPower, x.StateID, x.IsActive, x.CreatedBy, x.CreatedDate, x.LastModifiedBy, x.LastModifiedDate, Array.Empty<DailyLaborDetailModel>()))
            .ToArrayAsync(cancellationToken);
        return new PagedResult<DailyLaborModel>(items, totalCount);
    }

}

