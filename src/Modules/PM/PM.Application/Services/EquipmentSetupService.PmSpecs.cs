using Himapp.PM.Application.Features.EquipmentSetup.Models;
using Himapp.PM.Domain.Constants;
using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Services;

// Steps 5 (preventive maintenance), 6 (specifications), 7 (review) and 8 (completion).
public sealed partial class EquipmentSetupService
{
    // ------------------------------------------------------------------------------------------------ step 5: preventive maintenance

    private static bool IsKnownSource(string? source) =>
        source == EquipmentSetupSource.Category || source == EquipmentSetupSource.Master;

    public async Task<EquipmentPmStepDto> GetPmAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        return await BuildPmAsync(c, ct);
    }

    private async Task<EquipmentPmStepDto> BuildPmAsync(Ctx c, CancellationToken ct)
    {
        var equipmentId = c.Equipment.ID;
        var links = new List<EquipmentCategoryPMPoint>();
        if (c.Category is not null)
        {
            var categoryId = c.Category.ID;
            links = await _db.Set<EquipmentCategoryPMPoint>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && l.IsActive).OrderBy(l => l.ID).ToListAsync(ct);
        }
        var rows = await _db.Set<EquipmentSetupPMPoint>().AsNoTracking()
            .Where(r => r.EquipmentId == equipmentId && r.IsActive).OrderBy(r => r.ID).ToListAsync(ct);

        var pmIds = links.Select(l => l.PreventiveMaintenancePointId).Concat(rows.Select(r => r.PreventiveMaintenancePointId)).Distinct().ToList();
        var points = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking().Where(p => pmIds.Contains(p.ID)).ToListAsync(ct);
        var pointNames = points.ToDictionary(p => p.ID, p => p.MaintenancePointName ?? string.Empty);

        var readingIds = links.Where(l => l.ReadingTypeId.HasValue).Select(l => l.ReadingTypeId!.Value)
            .Concat(rows.Where(r => r.ReadingTypeId.HasValue).Select(r => r.ReadingTypeId!.Value)).Distinct().ToList();
        var readingTypes = await _db.Set<MeterReadingType>().AsNoTracking().Where(t => readingIds.Contains(t.ID)).ToListAsync(ct);
        var readingNames = readingTypes.ToDictionary(t => t.ID, t => t.ReadingTypeName);

        string NameOf(int pmId) => pointNames.TryGetValue(pmId, out var n) ? n : string.Empty;

        var categoryPoints = new List<EquipmentPmPointDto>();
        var masterPoints = new List<EquipmentPmPointDto>();
        var rowByPm = rows.ToDictionary(r => r.PreventiveMaintenancePointId);
        var linkByPm = links.ToDictionary(l => l.PreventiveMaintenancePointId);

        foreach (var link in links)
        {
            if (rowByPm.TryGetValue(link.PreventiveMaintenancePointId, out var row))
            {
                // Added by hand earlier: stays in the master list even though the category suggests it now.
                if (row.Source == EquipmentSetupSource.Master) continue;
                categoryPoints.Add(ToPmDto(NameOf(row.PreventiveMaintenancePointId), row, link, readingNames));
            }
            else
            {
                categoryPoints.Add(ToPmDto(NameOf(link.PreventiveMaintenancePointId), null, link, readingNames));
            }
        }

        foreach (var row in rows)
        {
            linkByPm.TryGetValue(row.PreventiveMaintenancePointId, out var link);
            if (row.Source == EquipmentSetupSource.Master)
                masterPoints.Add(ToPmDto(NameOf(row.PreventiveMaintenancePointId), row, link, readingNames));
            else if (link is null)
                categoryPoints.Add(ToPmDto(NameOf(row.PreventiveMaintenancePointId), row, null, readingNames)); // category no longer suggests it
        }

        var all = categoryPoints.Concat(masterPoints).ToList();
        var summary = new EquipmentPmSummaryDto(
            categoryPoints.Count(p => p.IsSelected), masterPoints.Count(p => p.IsSelected), all.Count(p => p.IsSelected), all.Count);
        return new EquipmentPmStepDto(c.Category?.ID, c.Category?.NAME, summary, categoryPoints, masterPoints);
    }

    /// <summary>One PM row. The default comes from the category link; the configured value from the equipment row (or the default when nothing is saved yet).</summary>
    private static EquipmentPmPointDto ToPmDto(string name, EquipmentSetupPMPoint? row, EquipmentCategoryPMPoint? link, Dictionary<int, string> readingNames)
    {
        var readingTypeId = row is not null ? row.ReadingTypeId : link?.ReadingTypeId;
        var meterConfigured = row is not null ? row.IntervalValue : link?.IntervalValue;
        var meterTolerance = row is not null ? row.ToleranceValue : link?.ToleranceValue;
        var monthsConfigured = row is not null ? row.Months : link?.Months;
        var monthsTolerance = row is not null ? row.MonthsTolerance : link?.MonthsTolerance;

        EquipmentPmTriggerDto? meter = null;
        if (readingTypeId.HasValue || meterConfigured.HasValue || link?.IntervalValue != null)
            meter = new EquipmentPmTriggerDto(link?.IntervalValue, link?.ToleranceValue, meterConfigured, meterTolerance);

        EquipmentPmTriggerDto? months = null;
        if (monthsConfigured.HasValue || link?.Months != null)
            months = new EquipmentPmTriggerDto(link?.Months, link?.MonthsTolerance, monthsConfigured, monthsTolerance);

        var pmId = row?.PreventiveMaintenancePointId ?? link!.PreventiveMaintenancePointId;
        var source = row?.Source ?? EquipmentSetupSource.Category;
        var readingName = readingTypeId.HasValue && readingNames.TryGetValue(readingTypeId.Value, out var rn) ? rn : null;
        return new EquipmentPmPointDto(pmId, name, source, row?.IsSelected ?? true, readingTypeId, readingName, meter, months, row?.Remarks, null);
    }

    public async Task<IReadOnlyList<EquipmentOptionDto>> GetPmAvailableAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        var step = await BuildPmAsync(c, ct);
        var used = step.CategoryPoints.Concat(step.MasterPoints).Select(p => p.PmPointId).ToHashSet();

        var masters = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking().Where(p => p.IsActive).ToListAsync(ct);
        return masters
            .Where(p => !used.Contains(p.ID))
            .Select(p => new EquipmentOptionDto(p.ID, p.MaintenancePointName ?? string.Empty))
            .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<EquipmentSetupStepResultDto> SavePmAsync(int equipmentId, EquipmentPmSaveRequest request, bool draft, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.PreventiveMaintenance);
        var items = request.Items ?? new List<EquipmentPmPointRequest>();

        var duplicate = items.GroupBy(i => i.PmPointId).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
            throw new ArgumentException($"PM point {duplicate.Key} is listed more than once.");

        var pmIds = items.Select(i => i.PmPointId).ToList();
        var masters = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking().Where(p => pmIds.Contains(p.ID)).ToListAsync(ct);
        var masterById = masters.ToDictionary(p => p.ID);

        var links = new Dictionary<int, EquipmentCategoryPMPoint>();
        if (c.Category is not null)
        {
            var categoryId = c.Category.ID;
            var list = await _db.Set<EquipmentCategoryPMPoint>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && l.IsActive).ToListAsync(ct);
            links = list.ToDictionary(l => l.PreventiveMaintenancePointId);
        }

        var readingIds = items.Where(i => i.MeterTrigger?.ReadingTypeId != null).Select(i => i.MeterTrigger!.ReadingTypeId!.Value).Distinct().ToList();
        var activeReadingTypes = (await _db.Set<MeterReadingType>().AsNoTracking().Where(t => readingIds.Contains(t.ID) && t.IsActive).ToListAsync(ct))
            .Select(t => t.ID).ToHashSet();

        var existing = await _db.Set<EquipmentSetupPMPoint>().Where(r => r.EquipmentId == equipmentId).ToListAsync(ct);
        var rowByPm = existing.ToDictionary(r => r.PreventiveMaintenancePointId);
        var now = DateTime.UtcNow;

        foreach (var i in items)
        {
            if (!IsKnownSource(i.Source))
                throw new ArgumentException($"Source must be '{EquipmentSetupSource.Category}' or '{EquipmentSetupSource.Master}'.");
            if (!masterById.TryGetValue(i.PmPointId, out var master))
                throw new ArgumentException($"PM point {i.PmPointId} was not found.");

            var hasRow = rowByPm.TryGetValue(i.PmPointId, out var row);
            if (!master.IsActive && !(hasRow && row!.IsActive))
                throw new ArgumentException($"PM point '{master.MaintenancePointName}' is not active.");

            links.TryGetValue(i.PmPointId, out var link);

            // Meter trigger: what was sent, otherwise the category default.
            int? readingTypeId; decimal? interval; decimal? tolerance;
            if (i.MeterTrigger is not null)
            {
                readingTypeId = i.MeterTrigger.ReadingTypeId;
                interval = i.MeterTrigger.ConfiguredValue;
                tolerance = i.MeterTrigger.ConfiguredTolerance;
                if (readingTypeId.HasValue != interval.HasValue)
                    throw new ArgumentException($"'{master.MaintenancePointName}': reading type and meter value must be given together.");
                if (interval.HasValue && interval.Value <= 0)
                    throw new ArgumentException($"'{master.MaintenancePointName}': meter value must be greater than 0.");
                if (readingTypeId.HasValue && !activeReadingTypes.Contains(readingTypeId.Value))
                    throw new ArgumentException($"'{master.MaintenancePointName}': the selected reading type was not found or is not active.");
            }
            else
            {
                readingTypeId = link?.ReadingTypeId;
                interval = link?.IntervalValue;
                tolerance = link?.ToleranceValue;
            }

            // Calendar trigger: what was sent, otherwise the category default.
            int? months; decimal? monthsTolerance;
            if (i.MonthsTrigger is not null)
            {
                var value = i.MonthsTrigger.ConfiguredValue;
                if (value.HasValue && (value.Value <= 0 || value.Value != Math.Truncate(value.Value)))
                    throw new ArgumentException($"'{master.MaintenancePointName}': months must be a whole number greater than 0.");
                months = value.HasValue ? (int)value.Value : null;
                monthsTolerance = i.MonthsTrigger.ConfiguredTolerance;
            }
            else
            {
                months = link?.Months;
                monthsTolerance = link?.MonthsTolerance;
            }

            if ((tolerance.HasValue && (tolerance.Value < 0 || tolerance.Value > 100)) ||
                (monthsTolerance.HasValue && (monthsTolerance.Value < 0 || monthsTolerance.Value > 100)))
                throw new ArgumentException($"'{master.MaintenancePointName}': tolerance must be between 0 and 100 (percent).");
            if (i.Remarks is { Length: > 500 })
                throw new ArgumentException("Remarks can be at most 500 characters.");
            if (!draft && i.IsSelected && !interval.HasValue && !months.HasValue)
                throw new ArgumentException($"'{master.MaintenancePointName}' is selected but has no trigger (meter value or months).");

            if (!hasRow)
            {
                row = new EquipmentSetupPMPoint { EquipmentId = equipmentId, PreventiveMaintenancePointId = i.PmPointId };
                StampNew(row, userId, now);
                _db.Set<EquipmentSetupPMPoint>().Add(row);
            }
            else
            {
                StampChanged(row!, userId, now);
            }

            row!.IsActive = true;
            row.Source = i.Source;
            row.IsSelected = i.IsSelected;
            row.ReadingTypeId = readingTypeId;
            row.IntervalValue = interval;
            row.ToleranceValue = tolerance;
            row.Months = months;
            row.MonthsTolerance = monthsTolerance;
            row.Remarks = Clean(i.Remarks);
        }

        // PM points that are no longer in the list are switched off (kept for history).
        var listed = pmIds.ToHashSet();
        foreach (var row in existing.Where(r => r.IsActive && !listed.Contains(r.PreventiveMaintenancePointId)))
        {
            row.IsActive = false;
            StampChanged(row, userId, now);
        }

        return await FinishStepAsync(c, EquipmentSetupStepNo.PreventiveMaintenance, draft, userId, now, ct);
    }

    public async Task<EquipmentPmStepDto> RestorePmDefaultsAsync(int equipmentId, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.PreventiveMaintenance);

        if (c.Category is not null)
        {
            var categoryId = c.Category.ID;
            var links = await _db.Set<EquipmentCategoryPMPoint>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && l.IsActive).ToListAsync(ct);
            var linkByPm = links.ToDictionary(l => l.PreventiveMaintenancePointId);
            var rows = await _db.Set<EquipmentSetupPMPoint>()
                .Where(r => r.EquipmentId == equipmentId && r.IsActive && r.Source == EquipmentSetupSource.Category).ToListAsync(ct);

            var now = DateTime.UtcNow;
            foreach (var row in rows)
            {
                if (!linkByPm.TryGetValue(row.PreventiveMaintenancePointId, out var link)) continue;
                row.IsSelected = true;
                row.ReadingTypeId = link.ReadingTypeId;
                row.IntervalValue = link.IntervalValue;
                row.ToleranceValue = link.ToleranceValue;
                row.Months = link.Months;
                row.MonthsTolerance = link.MonthsTolerance;
                StampChanged(row, userId, now);
            }
            await _db.SaveChangesAsync(ct);
        }

        return await BuildPmAsync(c, ct);
    }

    // ------------------------------------------------------------------------------------------------ step 6: specifications

    public async Task<EquipmentSpecificationsStepDto> GetSpecificationsAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        return await BuildSpecificationsAsync(c, ct);
    }

    private async Task<EquipmentSpecificationsStepDto> BuildSpecificationsAsync(Ctx c, CancellationToken ct)
    {
        var equipmentId = c.Equipment.ID;
        var links = new List<EquipmentCategorySpecification>();
        if (c.Category is not null)
        {
            var categoryId = c.Category.ID;
            links = await _db.Set<EquipmentCategorySpecification>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && l.IsActive).OrderBy(l => l.ID).ToListAsync(ct);
        }
        var rows = await _db.Set<EquipmentSetupSpecification>().AsNoTracking()
            .Where(r => r.EquipmentId == equipmentId && r.IsActive).OrderBy(r => r.ID).ToListAsync(ct);

        var ids = links.Select(l => l.SpecificationMasterId).Concat(rows.Select(r => r.SpecificationMasterId)).Distinct().ToList();
        var masters = await _db.Set<SpecificationMaster>().AsNoTracking().Where(m => ids.Contains(m.ID)).ToListAsync(ct);
        var masterById = masters.ToDictionary(m => m.ID);
        var rowBySpec = rows.ToDictionary(r => r.SpecificationMasterId);
        var linkBySpec = links.ToDictionary(l => l.SpecificationMasterId);

        EquipmentSpecificationDto Dto(int specId, EquipmentSetupSpecification? row, string source)
        {
            masterById.TryGetValue(specId, out var m);
            return new EquipmentSpecificationDto(specId, m?.SpecificationName ?? string.Empty, m?.Type ?? 0, source,
                row?.IsSelected ?? true, row?.Value, row?.Remarks);
        }

        var categorySpecs = new List<EquipmentSpecificationDto>();
        var masterSpecs = new List<EquipmentSpecificationDto>();

        foreach (var link in links)
        {
            if (rowBySpec.TryGetValue(link.SpecificationMasterId, out var row))
            {
                if (row.Source == EquipmentSetupSource.Master) continue;
                categorySpecs.Add(Dto(link.SpecificationMasterId, row, EquipmentSetupSource.Category));
            }
            else
            {
                categorySpecs.Add(Dto(link.SpecificationMasterId, null, EquipmentSetupSource.Category));
            }
        }

        foreach (var row in rows)
        {
            if (row.Source == EquipmentSetupSource.Master)
                masterSpecs.Add(Dto(row.SpecificationMasterId, row, EquipmentSetupSource.Master));
            else if (!linkBySpec.ContainsKey(row.SpecificationMasterId))
                categorySpecs.Add(Dto(row.SpecificationMasterId, row, EquipmentSetupSource.Category)); // category no longer suggests it
        }

        var all = categorySpecs.Concat(masterSpecs).ToList();
        var summary = new EquipmentSpecificationSummaryDto(
            categorySpecs.Count(s => s.IsSelected), masterSpecs.Count(s => s.IsSelected), all.Count(s => s.IsSelected), all.Count);
        return new EquipmentSpecificationsStepDto(c.Category?.ID, c.Category?.NAME, summary, categorySpecs, masterSpecs);
    }

    public async Task<IReadOnlyList<EquipmentSpecificationOptionDto>> GetSpecificationsAvailableAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        var step = await BuildSpecificationsAsync(c, ct);
        var used = step.CategorySpecifications.Concat(step.MasterSpecifications).Select(s => s.SpecificationId).ToHashSet();

        var masters = await _db.Set<SpecificationMaster>().AsNoTracking().Where(m => m.IsActive).ToListAsync(ct);
        return masters
            .Where(m => !used.Contains(m.ID))
            .Select(m => new EquipmentSpecificationOptionDto(m.ID, m.SpecificationName, m.Type))
            .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<EquipmentSetupStepResultDto> SaveSpecificationsAsync(int equipmentId, EquipmentSpecificationsSaveRequest request, bool draft, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.Specifications);
        var items = request.Items ?? new List<EquipmentSpecificationRequest>();

        var duplicate = items.GroupBy(i => i.SpecificationId).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
            throw new ArgumentException($"Specification {duplicate.Key} is listed more than once.");

        var ids = items.Select(i => i.SpecificationId).ToList();
        var masters = await _db.Set<SpecificationMaster>().AsNoTracking().Where(m => ids.Contains(m.ID)).ToListAsync(ct);
        var masterById = masters.ToDictionary(m => m.ID);

        var existing = await _db.Set<EquipmentSetupSpecification>().Where(r => r.EquipmentId == equipmentId).ToListAsync(ct);
        var rowBySpec = existing.ToDictionary(r => r.SpecificationMasterId);
        var now = DateTime.UtcNow;

        foreach (var i in items)
        {
            if (!IsKnownSource(i.Source))
                throw new ArgumentException($"Source must be '{EquipmentSetupSource.Category}' or '{EquipmentSetupSource.Master}'.");
            if (!masterById.TryGetValue(i.SpecificationId, out var master))
                throw new ArgumentException($"Specification {i.SpecificationId} was not found.");

            var hasRow = rowBySpec.TryGetValue(i.SpecificationId, out var row);
            if (!master.IsActive && !(hasRow && row!.IsActive))
                throw new ArgumentException($"Specification '{master.SpecificationName}' is not active.");

            var value = Clean(i.Value);
            if (value is { Length: > 500 })
                throw new ArgumentException($"The value of '{master.SpecificationName}' can be at most 500 characters.");
            if (i.Remarks is { Length: > 500 })
                throw new ArgumentException("Remarks can be at most 500 characters.");
            if (!draft && i.IsSelected && value is null)
                throw new ArgumentException($"A value is required for '{master.SpecificationName}'. Deselect it if it does not apply to this equipment.");

            if (!hasRow)
            {
                row = new EquipmentSetupSpecification { EquipmentId = equipmentId, SpecificationMasterId = i.SpecificationId };
                StampNew(row, userId, now);
                _db.Set<EquipmentSetupSpecification>().Add(row);
            }
            else
            {
                StampChanged(row!, userId, now);
            }

            row!.IsActive = true;
            row.Source = i.Source;
            row.IsSelected = i.IsSelected;
            row.Value = value;
            row.Remarks = Clean(i.Remarks);
        }

        var listed = ids.ToHashSet();
        foreach (var row in existing.Where(r => r.IsActive && !listed.Contains(r.SpecificationMasterId)))
        {
            row.IsActive = false;
            StampChanged(row, userId, now);
        }

        return await FinishStepAsync(c, EquipmentSetupStepNo.Specifications, draft, userId, now, ct);
    }

    // ------------------------------------------------------------------------------------------------ step 7: review, step 8: completion

    public async Task<EquipmentReviewDto> GetReviewAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        var pm = await BuildPmAsync(c, ct);
        var specs = await BuildSpecificationsAsync(c, ct);
        var epoch = await LatestEpochAsync(equipmentId, false, ct);
        var docs = await _db.Set<EquipmentSetupDocument>().AsNoTracking().Where(d => d.EquipmentId == equipmentId && d.IsActive).ToListAsync(ct);

        var selectedPm = pm.CategoryPoints.Concat(pm.MasterPoints).Where(p => p.IsSelected).ToList();
        var triggers = selectedPm.SelectMany(p => new[] { p.MeterTrigger, p.MonthsTrigger }).Where(t => t is not null).Select(t => t!).ToList();
        var overrides = triggers.Count(t => t.DefaultValue.HasValue && t.ConfiguredValue != t.DefaultValue);

        var meterStatus = !LogbookRequired(c) ? "Not required"
            : epoch?.OpeningReadingAmount is null ? "Pending" : $"Opening reading {epoch.OpeningReadingAmount}";
        var fuelStatus = !FuelRequired(c) ? "Not required"
            : c.Equipment.STDFUELCONSUMPTION_ISPL is null ? "Pending" : $"Standard {c.Equipment.STDFUELCONSUMPTION_ISPL}";

        return new EquipmentReviewDto(
            c.Equipment.OBJECTID, c.Equipment.NAME, c.Category?.NAME ?? c.Equipment.OBJECTTYPE, c.Equipment.FUNCTIONALLOCATION,
            ToInformation(c), meterStatus, fuelStatus,
            docs.Count, docs.Where(d => d.EndDate.HasValue).Select(d => d.EndDate).Max(),
            pm.Summary.CategorySelected, pm.Summary.FromMaster, triggers.Count, overrides,
            specs.Summary.Selected, BuildSteps(c));
    }

    /// <summary>"Complete equipment setup": steps 1 to 6 must be complete; marks steps 7 and 8 and the setup itself as completed.</summary>
    public async Task<EquipmentSetupStepResultDto> CompleteAsync(int equipmentId, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        if (c.Completed)
            throw new InvalidOperationException("The setup of this equipment is already completed.");

        var steps = BuildSteps(c);
        var open = steps.Where(s => s.StepNo <= EquipmentSetupStepNo.Specifications && s.Status != EquipmentSetupStepStatus.Complete).Select(s => s.Title).ToList();
        if (open.Count > 0)
            throw new InvalidOperationException("These steps are not complete yet: " + string.Join(", ", open) + ".");

        var now = DateTime.UtcNow;
        var setup = EnsureSetup(c, userId, now);
        foreach (var no in new[] { EquipmentSetupStepNo.Review, EquipmentSetupStepNo.Complete })
        {
            var step = EnsureStep(c, no, userId, now);
            step.Status = EquipmentSetupStepStatus.Complete;
            step.CompletedAt ??= now;
            StampChanged(step, userId, now);
        }
        setup.Status = EquipmentSetupStatus.Completed;
        setup.CurrentStep = EquipmentSetupStepNo.Complete;
        setup.CompletedAt = now;
        StampChanged(setup, userId, now);

        await _db.SaveChangesAsync(ct);
        return Result(c, EquipmentSetupStepNo.Complete);
    }

    public async Task<EquipmentCompletionDto> GetCompletionAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        var pm = await BuildPmAsync(c, ct);
        var epoch = await LatestEpochAsync(equipmentId, false, ct);

        var meter = !LogbookRequired(c) ? "Not required"
            : epoch?.OpeningReadingAmount is null ? "Pending" : $"Opening reading {epoch.OpeningReadingAmount}";
        var fuel = !FuelRequired(c) ? "Not required"
            : c.Equipment.STDFUELCONSUMPTION_ISPL is null ? "Pending" : $"Standard {c.Equipment.STDFUELCONSUMPTION_ISPL}";

        return new EquipmentCompletionDto(
            c.Equipment.OBJECTID,
            c.Setup?.Status ?? EquipmentSetupStatus.Draft,
            c.Setup?.CompletedAt,
            meter, fuel,
            pm.Summary.Selected, pm.Summary.CategorySelected, pm.Summary.FromMaster);
    }
}
