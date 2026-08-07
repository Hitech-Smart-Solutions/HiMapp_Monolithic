using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Himapp.Execution.Application.Features.Planning.Handlers;

internal sealed class PlanningHandlers :
    IRequestHandler<GetAllPlanningsQuery, IReadOnlyCollection<PlanningModel>>,
    IRequestHandler<GetPlanningByIdQuery, PlanningModel?>,
    IRequestHandler<CreatePlanningCommand, PlanningModel>,
    IRequestHandler<UpdatePlanningCommand, PlanningModel?>,
    IRequestHandler<DeletePlanningCommand, bool>,
    IRequestHandler<GetPlanningListByProjectQuery, DataSet>
{
    private readonly IExecutionDbContext _db;
    public PlanningHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<PlanningModel>> Handle(GetAllPlanningsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Select(p => new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.AreaID, p.PlanTypeID, p.StartDate, p.EndDate, p.Remarks, p.StatusID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate, Array.Empty<PlanningDetailModel>(), Array.Empty<PlanningDocumentDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PlanningModel?> Handle(GetPlanningByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Include(x => x.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;

        var details = p.PlanningDetail?.Select(pd => new PlanningDetailModel(
            pd.ID,
            pd.UniqueID,
            pd.AreaID,
            pd.ActivityID,
            pd.TargetQuantity,
            pd.UOMID,
            pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        var docDetails = p.PlanningDocumentDetail?.Select(pd => new PlanningDocumentDetailModel(
            pd.ID,
            pd.UniqueID,
            pd.DocumentName,
            pd.FileName,
            pd.FilePath,
            pd.FileExtension,
            pd.ContentType)).ToArray() ?? Array.Empty<PlanningDocumentDetailModel>();

        return new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.AreaID, p.PlanTypeID, p.StartDate, p.EndDate, p.Remarks, p.StatusID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate, details, docDetails);
    }

    public async Task<PlanningModel> Handle(CreatePlanningCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.Planning
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            AreaID = r.AreaID,
            PlanTypeID = r.PlanTypeID,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Remarks = r.Remarks,
            StatusID = 3,
            IsActive = true,
            CreatedBy = r.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = r.CreatedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.PlanningDetail
                {
                    UniqueID = Guid.NewGuid(),
                    AreaID = d.AreaId,
                    ActivityID = d.ActivityId,
                    TargetQuantity = d.TargetQuantity,
                    UOMID = d.UomId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.CreatedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.PlanningDetail?.Add(detail);
            }
        }

        if (r.docDetails?.Any() == true)
        {
            foreach (var d in r.docDetails)
            {
                var docDetail = new Himapp.Execution.Domain.Entities.PlanningDocumentDetail
                {
                    UniqueID = Guid.NewGuid(),
                    DocumentName = d.DocumentName,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    FileExtension = d.FileExtension,
                    ContentType = d.ContentType,
                    IsActive = true,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.CreatedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.PlanningDocumentDetail?.Add(docDetail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.Planning>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.PlanningDetail?.Select(pd => new PlanningDetailModel(pd.ID, pd.UniqueID, pd.AreaID, pd.ActivityID, pd.TargetQuantity, pd.UOMID, pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        var docDetails = entity.PlanningDocumentDetail?.Select(pd => new PlanningDocumentDetailModel(pd.ID, pd.UniqueID, pd.DocumentName, pd.FileName, pd.FilePath, pd.FileExtension, pd.ContentType)).ToArray() ?? Array.Empty<PlanningDocumentDetailModel>();

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.AreaID, entity.PlanTypeID, entity.StartDate, entity.EndDate, entity.Remarks, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, docDetails);
    }

    public async Task<PlanningModel?> Handle(UpdatePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .Include(d => d.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        int LastModifiedBy = request.Request.LastModifiedBy;

        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.StatusID = request.Request.StatusID;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedBy = LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details and add new ones
        if (entity.PlanningDetail != null && entity.PlanningDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.PlanningDetail>().RemoveRange(entity.PlanningDetail);
            entity.PlanningDetail.Clear();
        }

        if (request.Request.Details?.Any() == true)
        {
            foreach (var d in request.Request.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.PlanningDetail
                {
                    UniqueID = Guid.NewGuid(),
                    AreaID = d.AreaId,
                    ActivityID = d.ActivityId,
                    TargetQuantity = d.TargetQuantity,
                    UOMID = d.UomId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.PlanningDetail?.Add(detail);
            }
        }

        // Remove existing document details and add new ones
        if (entity.PlanningDocumentDetail != null && entity.PlanningDocumentDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.PlanningDocumentDetail>().RemoveRange(entity.PlanningDocumentDetail);
            entity.PlanningDocumentDetail.Clear();
        }

        if (request.Request.docDetails?.Any() == true)
        {
            foreach (var d in request.Request.docDetails)
            {
                var docDetail = new Himapp.Execution.Domain.Entities.PlanningDocumentDetail
                {
                    UniqueID = Guid.NewGuid(),
                    DocumentName = d.DocumentName,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    FileExtension = d.FileExtension,
                    ContentType = d.ContentType,
                    IsActive = true,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.PlanningDocumentDetail?.Add(docDetail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.PlanningDetail?.Select(pd => new PlanningDetailModel(pd.ID, pd.UniqueID, pd.AreaID, pd.ActivityID, pd.TargetQuantity, pd.UOMID, pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        var docDetails = entity.PlanningDocumentDetail?.Select(pd => new PlanningDocumentDetailModel(pd.ID, pd.UniqueID, pd.DocumentName, pd.FileName, pd.FilePath, pd.FileExtension, pd.ContentType)).ToArray() ?? Array.Empty<PlanningDocumentDetailModel>();

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.AreaID, entity.PlanTypeID, entity.StartDate, entity.EndDate, entity.Remarks, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, docDetails);
    }

    public async Task<bool> Handle(DeletePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .Include(d => d.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = 0;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.PlanningDetail != null)
        {
            foreach (var pd in entity.PlanningDetail)
            {
                pd.IsActive = false;
                pd.LastModifiedBy = request.DeletedBy;
                pd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        if (entity.PlanningDocumentDetail != null)
        {
            foreach (var pd in entity.PlanningDocumentDetail)
            {
                pd.IsActive = false;
                pd.LastModifiedBy = request.DeletedBy;
                pd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DataSet> Handle(GetPlanningListByProjectQuery request, CancellationToken cancellationToken)
    {
        DataSet ds = new DataSet();

        string connectionString =
            _db.Database.GetDbConnection().ConnectionString;

        #region LIST

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM execution.uspgetplanningbyprojectid(
                @p_projectid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_projectid",
                request.SearchParams.ProjectID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        #region COUNT

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM execution.uspgetplanningcountbyprojectid(
                @p_projectid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_projectid",
                request.SearchParams.ProjectID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        return ds;
    }
}

