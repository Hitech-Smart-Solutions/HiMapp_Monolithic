using ClosedXML.Excel;
using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Application.Features.Planning.Services;
using Himapp.Execution.Application.Features.Planning.Services.IServices;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq;
using System.Data;

namespace Himapp.Execution.Application.Features.Planning.Handlers;

internal sealed class PlanningHandlers :
    IRequestHandler<GetAllPlanningsQuery, IReadOnlyCollection<PlanningModel>>,
    IRequestHandler<GetPlanningByIdQuery, PlanningModel?>,
    IRequestHandler<CreatePlanningCommand, PlanningModel>,
    IRequestHandler<UpdatePlanningCommand, PlanningModel?>,
    IRequestHandler<DeletePlanningCommand, bool>,
    IRequestHandler<GetPlanningListByProjectQuery, DataSet>,
    IRequestHandler<BulkCreatePlanningCommand, IReadOnlyCollection<PlanningModel>>,
    IRequestHandler<DownloadPlanningTemplateQuery, byte[]>
{
    private readonly IExecutionDbContext _db;
    private readonly Himapp.Files.Services.IFileService _fileService;
    private readonly IExcelPlanningImporter _excelImporter;
    private readonly IPlanningSectionService _planningSectionService;

    public PlanningHandlers(IExecutionDbContext db, Himapp.Files.Services.IFileService fileService, IExcelPlanningImporter excelImporter, IPlanningSectionService planningSectionService) => (_db, _fileService, _excelImporter, _planningSectionService) = (db, fileService, excelImporter, planningSectionService);

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
            .Include(x => x.PlanningDocumentDetail)
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
            .Include(x => x.PlanningDocumentDetail)
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
            .Include(x => x.PlanningDocumentDetail)
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
                @"SELECT * FROM execution.uspgetexecutionplanningbyprojectid(
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
                @"SELECT * FROM execution.uspgetexecutionplanningcountbyprojectid(
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

    public async Task<IReadOnlyCollection<PlanningModel>> Handle(BulkCreatePlanningCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        // Parse excel and validate
        var parseResult = await _excelImporter.ParseAsync(r.ExcelFile, r.ProjectId, cancellationToken);
        if (parseResult.Errors.Any())
        {
            throw new InvalidOperationException(string.Join("||", parseResult.Errors));
        }

        // Handle attachment via file service (register once and reuse for all plannings)
        List<PlanningDocumentDetailRequest>? docDetails = null;
        if (r.Attachment is not null && r.Attachment.Length > 0)
        {
            var fileAsset = await _fileService.RegisterAsync(r.Attachment.FileName, r.Attachment.ContentType ?? string.Empty, "planning.attachment", (int)r.Attachment.Length, cancellationToken);
            docDetails = new List<PlanningDocumentDetailRequest>
            {
                new PlanningDocumentDetailRequest
                {
                    DocumentName = r.Attachment.FileName,
                    FileName = fileAsset.FileName,
                    FilePath = fileAsset.StorageKey,
                    FileExtension = System.IO.Path.GetExtension(fileAsset.FileName),
                    ContentType = r.Attachment.ContentType
                }
            };
        }

        // Group details by AreaId — only include positive TargetQuantity (defensive)
        var groups = parseResult.Details
            .Where(d => d.TargetQuantity > 0)
            .GroupBy(d => d.AreaId)
            .ToList();
        var created = new List<PlanningModel>();

        // Use a transaction to ensure all-or-nothing behavior
        var dbContext = _db as DbContext;
        if (dbContext is null)
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext. Cannot start transaction.");

        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var g in groups)
            {
                var areaId = g.Key;
                var detailsForArea = g.Where(d => d.TargetQuantity > 0).ToList();

                if (detailsForArea.Count == 0)
                {
                    // nothing valid for this area
                    continue;
                }

                var createReq = new CreatePlanningRequest(r.ProjectId, areaId, r.PlanTypeID, r.StartDate, r.EndDate, r.Remarks, r.CreatedBy, detailsForArea, docDetails);
                var createdModel = await Handle(new CreatePlanningCommand(createReq), cancellationToken);
                created.Add(createdModel);
            }

            await tx.CommitAsync(cancellationToken);
            return created;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<byte[]> Handle(DownloadPlanningTemplateQuery request, CancellationToken cancellationToken)
    {
        // Load sections for project using injected IPlanningSectionService
        var sectionsList = (await _planningSectionService.GetProjectSectionsAsync(request.ProjectId, cancellationToken)).ToList();
        if (sectionsList == null || !sectionsList.Any())
            throw new InvalidOperationException("No active sections configured for this project.");

        var activities = new List<(int ActivityId, string ActivityName, int UomId, decimal? Rate)>();

        // Use existing ProjectActivity mapping stored in ProjectActivity table to fetch activities for the project
        var paList = await _db.Set<ProjectActivity>()
            .AsNoTracking()
            .Where(x => x.ProjectID == request.ProjectId && x.IsActive)
            .ToListAsync(cancellationToken);

        if (!paList.Any())
            throw new InvalidOperationException("No applicable activities found for this project.");

        // Map to activity master to get activity name and UOM
        var activityIds = paList.Select(x => x.ActivityID).Distinct().ToArray();
        var activityMasters = await _db.Set<Activity>()
            .AsNoTracking()
            .Where(a => activityIds.Contains(a.ID) && a.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var pa in paList)
        {
            var am = activityMasters.FirstOrDefault(a => a.ID == pa.ActivityID);
            if (am is null) continue;
            activities.Add((am.ID, am.ActivityName, am.UOMID, pa.RevenueRate));
        }

        // Build Excel using ClosedXML
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Template");

        // Two header rows
        ws.Cell(1, 1).Value = "Activities";
        ws.Cell(1, 2).Value = "Section Wise Targeted Quantity";
        ws.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(1, 2, 1, 1 + sectionsList.Count).Merge();

        ws.Cell(1, 2 + sectionsList.Count).Value = "UOM";
        ws.Cell(1, 3 + sectionsList.Count).Value = "Rate";
        //ws.Cell(1, 2 + sectionsList.Count + 1).Value = "UOM";
        //ws.Cell(1, 2 + sectionsList.Count + 2).Value = "Rate";

        // Row 2: area names
        ws.Cell(2, 1).Value = string.Empty;
        for (int i = 0; i < sectionsList.Count; i++)
        {
            var label = !string.IsNullOrWhiteSpace(sectionsList[i].LabelName) ? sectionsList[i].LabelName : sectionsList[i].SectionName;
            ws.Cell(2, 2 + i).Value = label;
        }

        ws.Cell(2, 2 + sectionsList.Count).Value = "UOM";
        ws.Cell(2, 3 + sectionsList.Count).Value = "Rate";
        //ws.Cell(2, 2 + sectionsList.Count + 1).Value = "UOM";
        //ws.Cell(2, 2 + sectionsList.Count + 2).Value = "Rate";

        // Activities start from row 3
        int row = 3;
        foreach (var act in activities)
        {
            ws.Cell(row, 1).Value = act.ActivityName;
            // leave area qty cells blank
        for (int i = 0; i < sectionsList.Count; i++)
            {
            ws.Cell(row, 2 + i).Value = string.Empty;
            }
            // UOM: try to get UOM name from UOMID - if not available leave blank
            string uomText = string.Empty;
            if (act.UomId > 0)
            {
                //var uom = await _db.Set<UOM>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == act.UomId, cancellationToken);
                //var uom = await _db.Set<object>().FindAsync(cancellationToken);
                // no UOM entity in project; leave blank
            }

            //ws.Cell(row, 2 + sectionsList.Count + 1).Value = string.Empty; // UOM text
            //ws.Cell(row, 2 + sectionsList.Count + 2).Value = act.Rate > 0 ? act.Rate : (decimal?)null;
            ws.Cell(row, 2 + sectionsList.Count).Value = string.Empty;
            ws.Cell(row, 3 + sectionsList.Count).Value = act.Rate > 0 ? act.Rate : (decimal?)null;

            row++;
        }

        // Formatting
        var headerRange = ws.Range(1, 1, 2, 2 + sectionsList.Count + 2);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        ws.SheetView.FreezeRows(2);
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}

