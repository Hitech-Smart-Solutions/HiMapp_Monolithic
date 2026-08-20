using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
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
using System.Data;
using System.Linq;
using PlanningEntity = Himapp.Execution.Domain.Entities.Planning;

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
        return await _db.Set<PlanningEntity>()
            .AsNoTracking()
            .Select(p => new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.AreaID, p.PlanTypeID, p.StartDate, p.EndDate, p.Remarks, p.StatusID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate, Array.Empty<PlanningDetailModel>(), Array.Empty<PlanningDocumentDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PlanningModel?> Handle(GetPlanningByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<PlanningEntity>()
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
        var entity = new PlanningEntity
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
                var detail = new PlanningDetail
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
                var docDetail = new PlanningDocumentDetail
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

        _db.Set<PlanningEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.PlanningDetail?.Select(pd => new PlanningDetailModel(pd.ID, pd.UniqueID, pd.AreaID, pd.ActivityID, pd.TargetQuantity, pd.UOMID, pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        var docDetails = entity.PlanningDocumentDetail?.Select(pd => new PlanningDocumentDetailModel(pd.ID, pd.UniqueID, pd.DocumentName, pd.FileName, pd.FilePath, pd.FileExtension, pd.ContentType)).ToArray() ?? Array.Empty<PlanningDocumentDetailModel>();

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.AreaID, entity.PlanTypeID, entity.StartDate, entity.EndDate, entity.Remarks, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, docDetails);
    }

    public async Task<PlanningModel?> Handle(UpdatePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<PlanningEntity>()
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
            _db.Set<PlanningDetail>().RemoveRange(entity.PlanningDetail);
            entity.PlanningDetail.Clear();
        }

        if (request.Request.Details?.Any() == true)
        {
            foreach (var d in request.Request.Details)
            {
                var detail = new PlanningDetail
                {
                    UniqueID = Guid.NewGuid(),
                    AreaID = d.AreaId,
                    ActivityID = d.ActivityId,
                    TargetQuantity = d.TargetQuantity,
                    UOMID = d.UomId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.PlanningDetail?.Add(detail);
            }
        }

        // Remove existing document details and add new ones
        if (entity.PlanningDocumentDetail != null && entity.PlanningDocumentDetail.Any())
        {
            _db.Set<PlanningDocumentDetail>().RemoveRange(entity.PlanningDocumentDetail);
            entity.PlanningDocumentDetail.Clear();
        }

        if (request.Request.docDetails?.Any() == true)
        {
            foreach (var d in request.Request.docDetails)
            {
                var docDetail = new PlanningDocumentDetail
                {
                    UniqueID = Guid.NewGuid(),
                    DocumentName = d.DocumentName,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    FileExtension = d.FileExtension,
                    ContentType = d.ContentType,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
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
        var entity = await _db.Set<PlanningEntity>()
            .Include(d => d.PlanningDetail)
            .Include(x => x.PlanningDocumentDetail)
            .FirstOrDefaultAsync(x => x.ID == request.dtoInactive.ProgramRowId, cancellationToken);
        if (entity is null) return false;

        bool isActive = request.dtoInactive.Actions == Actions.Activated;

        // Soft delete header and child details
        entity.IsActive = isActive;
        entity.LastModifiedBy = request.dtoInactive.UserId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.PlanningDetail != null)
        {
            foreach (var pd in entity.PlanningDetail)
            {
                pd.IsActive = isActive;
                pd.LastModifiedBy = request.dtoInactive.UserId;
                pd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        if (entity.PlanningDocumentDetail != null)
        {
            foreach (var pd in entity.PlanningDocumentDetail)
            {
                pd.IsActive = isActive;
                pd.LastModifiedBy = request.dtoInactive.UserId;
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
        try
        {
            // Load sections
            var sections = await _planningSectionService.GetProjectSectionsAsync(request.ProjectId, cancellationToken);

            if (sections == null || !sections.Any())
            {
                throw new InvalidOperationException("No active sections configured for this project.");
            }

            var sectionsList = sections.ToList();

            var activities = new List<(int ActivityId, string ActivityName, int UomId, string UomName, decimal? Rate)>();

            // Get project activities
            var paList = await _db.Set<ProjectActivity>()
                .AsNoTracking()
                .Where(x => x.ProjectID == request.ProjectId && x.IsActive)
                .ToListAsync(cancellationToken);

            if (!paList.Any())
            {
                throw new InvalidOperationException("No applicable activities found for this project.");
            }

            // Get activity IDs
            var activityIds = paList.Select(x => x.ActivityID).Distinct().ToList();

            if (!activityIds.Any())
            {
                throw new InvalidOperationException($"No Activity IDs found for project {request.ProjectId}.");
            }

            // Get activity masters
            var activityMasters = await _db.Set<Activity>()
                .AsNoTracking()
                .Where(a => activityIds.Contains(a.ID) && a.IsActive)
                .ToListAsync(cancellationToken);

            // GET UOM NAMES
            if (!activityMasters.Any())
            {
                throw new InvalidOperationException($"No active activities found for project {request.ProjectId}.");
            }

            // Get UOM IDs from activities
            var uomIds = activityMasters
                .Where(a => a.UOMID > 0)
                .Select(a => a.UOMID)
                .Distinct()
                .ToArray();

            var uomMap = new Dictionary<int, string>();

            if (uomIds.Length > 0)
            {
                var dbContext = _db as DbContext;

                if (dbContext is null)
                {
                    throw new InvalidOperationException("IExecutionDbContext is not a DbContext.");
                }

                var connection = dbContext.Database.GetDbConnection();

                await connection.OpenAsync(cancellationToken);

                try
                {
                    using var command = connection.CreateCommand();

                    command.CommandText = @"SELECT ""ID"", ""UOMShortName""
                        FROM public.""UnitOfMeasurement""
                        WHERE ""ID"" = ANY(@uomIds)";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@uomIds";
                    parameter.Value = uomIds;

                    command.Parameters.Add(parameter);

                    using var reader = await command.ExecuteReaderAsync(cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var id = reader.GetInt32(0);

                        var name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                        uomMap[id] = name;
                    }
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Map activities
            foreach (var pa in paList)
            {
                var am = activityMasters.FirstOrDefault(a => a.ID == pa.ActivityID);

                if (am is null) continue;

                string uomName = string.Empty;

                if (am.UOMID > 0)
                {
                    uomMap.TryGetValue(am.UOMID, out uomName);
                }

                activities.Add((am.ID, am.ActivityName, am.UOMID, uomName ?? string.Empty, pa.RevenueRate));
            }

            // Build Excel using ClosedXML
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Template");

            // HEADER ROW 1
            ws.Cell(1, 1).Value = "Activities";

            var sectionHeader = ws.Range(1, 2, 1, 1 + sectionsList.Count);
            sectionHeader.Merge();
            sectionHeader.FirstCell().Value = "Section Wise Targeted Quantity";

            // Merge section header
            ws.Range(1, 2, 1, 1 + sectionsList.Count).Merge();

            // UOM and Rate
            ws.Cell(1, 2 + sectionsList.Count).Value = "UOM";
            ws.Cell(1, 3 + sectionsList.Count).Value = "Rate";

            // HEADER ROW 2
            ws.Cell(2, 1).Value = string.Empty;

            for (int i = 0; i < sectionsList.Count; i++)
            {
                ws.Cell(2, 2 + i).Value = sectionsList[i].SectionName;
            }

            ws.Cell(2, 2 + sectionsList.Count).Value = string.Empty;
            ws.Cell(2, 3 + sectionsList.Count).Value = string.Empty;


            // DATA ROWS
            int row = 3;

            foreach (var act in activities)
            {
                ws.Cell(row, 1).Value = act.ActivityName;

                // Area quantity columns
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    ws.Cell(row, 2 + i).Value = string.Empty;
                }

                // UOM
                ws.Cell(row, 2 + sectionsList.Count).Value = act.UomName;

                // Rate
                ws.Cell(row, 3 + sectionsList.Count).Value = act.Rate > 0 ? act.Rate : (decimal?)null;

                row++;
            }

            // =============================
            // FORMATTING
            // =============================
            // Activity + Sections + UOM + Rate
            int totalColumns = 2 + sectionsList.Count + 1;

            // Last data row
            int lastRow = row - 1;

            // Apply bold + center alignment to headers
            var headerRange = ws.Range(1, 1, 2, totalColumns);

            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Apply borders to ENTIRE table
            var tableRange = ws.Range(1, 1, lastRow, totalColumns);

            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Center section quantity, UOM and Rate columns
            if (sectionsList.Count > 0)
            {
                var sectionRange = ws.Range(2, 2, lastRow, 1 + sectionsList.Count);

                sectionRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sectionRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // UOM column
            var uomRange = ws.Range(2, 2 + sectionsList.Count, lastRow, 2 + sectionsList.Count);

            uomRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            uomRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Rate column
            var rateRange = ws.Range(2, 3 + sectionsList.Count, lastRow, 3 + sectionsList.Count);

            rateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rateRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Freeze header rows
            ws.SheetView.FreezeRows(2);

            // Adjust column widths
            ws.Columns().AdjustToContents();

            ws.SheetView.FreezeRows(2);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
        catch (InvalidOperationException)
        {
            // Preserve business validation errors
            throw;
        }
        catch (Exception ex)
        {
            // IMPORTANT: log the actual exception and inner exception
            var innerMessage = ex.InnerException?.Message;

            throw new InvalidOperationException(
                $"Error while downloading planning template for ProjectId {request.ProjectId}. " +
                $"Error: {ex.Message}" +
                (!string.IsNullOrWhiteSpace(innerMessage)
                    ? $" | Inner Exception: {innerMessage}"
                    : string.Empty), ex);
        }
    }
}

