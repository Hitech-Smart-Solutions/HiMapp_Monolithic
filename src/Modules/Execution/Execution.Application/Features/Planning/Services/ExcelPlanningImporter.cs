using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Himapp.Execution.Application.Features.Planning.Services.IServices;

namespace Himapp.Execution.Application.Features.Planning.Services;

internal sealed class ExcelPlanningImporter : IExcelPlanningImporter
{

    private readonly IExecutionDbContext _db;
    private readonly IPlanningSectionService _sectionService;

    public ExcelPlanningImporter(IExecutionDbContext db, IPlanningSectionService sectionService)
    {
        _db = db;
        _sectionService = sectionService;
    }

    public async Task<PlanningImportParseResult> ParseAsync(Microsoft.AspNetCore.Http.IFormFile file, int projectId, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var details = new List<PlanningDetailRequest>();

        if (file is null || file.Length == 0)
        {
            errors.Add("Excel file is required.");
            return new PlanningImportParseResult(details, errors);
        }

        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var stream = file.OpenReadStream();
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var ds = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
        });

        if (ds.Tables.Count == 0)
        {
            errors.Add("Excel file contains no worksheets.");
            return new PlanningImportParseResult(details, errors);
        }

        var table = ds.Tables[0];

        if (table.Rows.Count < 3)
        {
            errors.Add("Excel file must contain two header rows and at least one data row.");
            return new PlanningImportParseResult(details, errors);
        }

        // Identify columns: first = Activity, last two = UOM, Rate. Middle columns = Areas
        if (table.Columns.Count < 4)
        {
            errors.Add("Excel file must contain at least Activity, one Area column, UOM and Rate columns.");
            return new PlanningImportParseResult(details, errors);
        }

        const int activityCol = 0;
        const int headerRow1 = 0;
        const int headerRow2 = 1;
        const int firstDataRow = 2;    // Excel row 3

        int uomCol = table.Columns.Count - 2;
        int rateCol = table.Columns.Count - 1;

        // area columns indexes
        var areaCols = Enumerable.Range(activityCol + 1, uomCol - (activityCol + 1)).ToArray();
        var areaNames = areaCols.Select(i => table.Rows[headerRow2][i]?.ToString()?.Trim() ?? string.Empty).ToArray();
        //var areaNames = areaCols.Select(i => table.Columns[i].ColumnName?.Trim() ?? string.Empty).ToArray();

        // Preload activities dictionary (case-insensitive)
        var activities = await _db.Set<Activity>().AsNoTracking().ToListAsync(cancellationToken);
        var activityMap = activities.ToDictionary(a => a.ActivityName.Trim().ToLowerInvariant(), a => a);

        // Build section/area lookup for the project once using the injected PlanningSectionService
        var sectionMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var sections = await _sectionService.GetProjectSectionsAsync(projectId, cancellationToken);
        foreach (var s in sections)
        {
            if (!string.IsNullOrWhiteSpace(s.SectionName) && !sectionMap.ContainsKey(s.SectionName.Trim()))
                sectionMap[s.SectionName.Trim()] = s.Id;
            if (!string.IsNullOrWhiteSpace(s.LabelName) && !sectionMap.ContainsKey(s.LabelName.Trim()))
                sectionMap[s.LabelName.Trim()] = s.Id;
        }

        for (int r = firstDataRow; r < table.Rows.Count; r++)
        {
            var row = table.Rows[r];
            var rowNumber = r + 1;      // Excel row number = DataTable index + 1

            var activityNameRaw = row[activityCol]?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(activityNameRaw))
            {
                errors.Add($"Row {rowNumber}: Activity is required.");
                continue;
            }

            if (!activityMap.TryGetValue(activityNameRaw.ToLowerInvariant(), out var activity))
            {
                errors.Add($"Row {rowNumber}: Activity '{activityNameRaw}' not found in system.");
                continue;
            }

            // UOM column is optional — prefer activity.UOMID
            int uomId = activity.UOMID;

            // Rate validation (not required for saving PlanningDetail) but validate numeric if present
            var rateRaw = row[rateCol]?.ToString()?.Trim();
            if (!string.IsNullOrWhiteSpace(rateRaw) && !decimal.TryParse(rateRaw, out _))
            {
                errors.Add($"Row {rowNumber}: Rate '{rateRaw}' is not a valid number.");
            }

            // For each area column, read target quantity
            foreach (var colIndex in areaCols)
            {
                var areaLabel =
                    table.Rows[headerRow2][colIndex]
                        ?.ToString()
                        ?.Trim() ?? string.Empty;

                // Ignore empty area headers
                if (string.IsNullOrWhiteSpace(areaLabel))
                {
                    continue;
                }

                var cellRaw =
                    row[colIndex]?.ToString()?.Trim() ?? string.Empty;

                // "-" or blank means no PlanningDetail
                if (cellRaw == "-" || string.IsNullOrWhiteSpace(cellRaw))
                {
                    continue;
                }

                if (!decimal.TryParse(cellRaw, out var targetQty))
                {
                    errors.Add(
                        $"Row {rowNumber}, Column '{areaLabel}': " +
                        $"Target Quantity '{cellRaw}' is not numeric.");

                    continue;
                }

                // Business rule: ignore non-positive quantities (<= 0)
                if (targetQty <= 0m)
                {
                    // Do not create PlanningDetail for zero/negative/blank quantities
                    continue;
                }

                // Resolve Area using preloaded sectionMap (both SectionName and LabelName were loaded)
                int? areaId = null;
                if (sectionMap.Count > 0 && !string.IsNullOrWhiteSpace(areaLabel))
                {
                    if (sectionMap.TryGetValue(areaLabel.Trim(), out var foundId))
                    {
                        areaId = foundId;
                    }
                }

                if (!areaId.HasValue)
                {
                    errors.Add($"Row {rowNumber}: Area '{areaLabel}' not found in system.");
                    continue;
                }

                var pd = new PlanningDetailRequest
                {
                    AreaId = areaId.Value,
                    ActivityId = activity.ID,
                    TargetQuantity = targetQty,
                    UomId = uomId,
                    Remarks = null
                };

                details.Add(pd);
            }

            // Old fallback area lookup removed. Area resolution uses preloaded sectionMap only.
        }

        return new PlanningImportParseResult(details, errors);
    }
}
