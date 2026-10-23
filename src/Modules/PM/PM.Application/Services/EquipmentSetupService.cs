using Himapp.PM.Application.Features.DynamicsEquipmentMasters;
using Himapp.PM.Application.Features.EquipmentSetup.Models;
using Himapp.PM.Domain.Constants;
using Himapp.PM.Domain.Constants.Enums;
using Himapp.PM.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Services;

/// <summary>
/// The 8-step equipment setup wizard. The D365 equipment master is the base; the equipment category only SUGGESTS PM points and
/// specifications; what the user keeps is stored against the equipment (EquipmentSetupPMPoint / EquipmentSetupSpecification / EquipmentSetupDocument).
/// Errors: KeyNotFoundException = not found (404), ArgumentException = bad input (400),
/// InvalidOperationException = step locked or setup already completed (409), UnauthorizedAccessException = no user in the token (401).
/// </summary>
public interface IEquipmentSetupService
{
    Task<EquipmentSetupOverviewDto> GetOverviewAsync(int equipmentId, CancellationToken ct);

    Task<EquipmentDetailsStepDto> GetDetailsAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> ConfirmDetailsAsync(int equipmentId, CancellationToken ct);

    Task<EquipmentInformationDto> GetInformationAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> SaveInformationAsync(int equipmentId, EquipmentInformationRequest request, bool draft, CancellationToken ct);

    Task<EquipmentMeterFuelDto> GetMeterFuelAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> SaveMeterFuelAsync(int equipmentId, EquipmentMeterFuelRequest request, bool draft, CancellationToken ct);

    Task<EquipmentDocumentsStepDto> GetDocumentsAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> SaveDocumentsAsync(int equipmentId, EquipmentDocumentsSaveRequest request, bool draft, CancellationToken ct);

    Task<EquipmentPmStepDto> GetPmAsync(int equipmentId, CancellationToken ct);
    Task<IReadOnlyList<EquipmentOptionDto>> GetPmAvailableAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> SavePmAsync(int equipmentId, EquipmentPmSaveRequest request, bool draft, CancellationToken ct);
    Task<EquipmentPmStepDto> RestorePmDefaultsAsync(int equipmentId, CancellationToken ct);

    Task<EquipmentSpecificationsStepDto> GetSpecificationsAsync(int equipmentId, CancellationToken ct);
    Task<IReadOnlyList<EquipmentSpecificationOptionDto>> GetSpecificationsAvailableAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> SaveSpecificationsAsync(int equipmentId, EquipmentSpecificationsSaveRequest request, bool draft, CancellationToken ct);

    Task<EquipmentReviewDto> GetReviewAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentSetupStepResultDto> CompleteAsync(int equipmentId, CancellationToken ct);
    Task<EquipmentCompletionDto> GetCompletionAsync(int equipmentId, CancellationToken ct);
}

public sealed partial class EquipmentSetupService : IEquipmentSetupService
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;

    public EquipmentSetupService(IPMDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    // ------------------------------------------------------------------------------------------------ wizard plumbing

    private sealed record StepDef(int No, string Key, string Title, string Hint);

    private static readonly StepDef[] StepDefs =
    {
        new(EquipmentSetupStepNo.Details, "details", "Review equipment details", "Read-only · D365"),
        new(EquipmentSetupStepNo.Information, "information", "Update equipment information", "Editable in HiMapp"),
        new(EquipmentSetupStepNo.MeterFuel, "meter-fuel", "Meter & fuel", "Category-based requirements"),
        new(EquipmentSetupStepNo.Documents, "documents", "Documents", "Optional"),
        new(EquipmentSetupStepNo.PreventiveMaintenance, "preventive-maintenance", "Preventive maintenance", "Engineer / In-Charge"),
        new(EquipmentSetupStepNo.Specifications, "specifications", "Specifications", "From equipment category"),
        new(EquipmentSetupStepNo.Review, "review", "Review setup", "P&M In-Charge"),
        new(EquipmentSetupStepNo.Complete, "complete", "Setup complete", string.Empty),
    };

    /// <summary>Everything one call needs: the equipment, its category (matched on OBJECTTYPE + DataAreaId), the wizard rows.</summary>
    private sealed class Ctx
    {
        public DynamicsEquipmentMaster Equipment { get; init; } = null!;
        public DynamicsEquipmentCategory? Category { get; init; }
        public EquipmentSetup? Setup { get; set; }
        public List<EquipmentSetupStep> Steps { get; init; } = new();
        public bool Completed => Setup?.Status == EquipmentSetupStatus.Completed;
    }

    private async Task<Ctx> LoadAsync(int equipmentId, CancellationToken ct)
    {
        var equipment = await _db.Set<DynamicsEquipmentMaster>().FirstOrDefaultAsync(e => e.ID == equipmentId, ct);
        if (equipment is null || !equipment.IsActive)
            throw new KeyNotFoundException($"Equipment {equipmentId} was not found.");

        DynamicsEquipmentCategory? category = null;
        if (!string.IsNullOrWhiteSpace(equipment.OBJECTTYPE))
        {
            var objectType = equipment.OBJECTTYPE;
            var dataAreaId = equipment.DataAreaId;
            category = await _db.Set<DynamicsEquipmentCategory>().AsNoTracking()
                .FirstOrDefaultAsync(c => c.OBJECTTYPEID == objectType && c.DataAreaId == dataAreaId, ct);
        }

        var setup = await _db.Set<EquipmentSetup>().FirstOrDefaultAsync(s => s.EquipmentId == equipmentId, ct);
        var steps = await _db.Set<EquipmentSetupStep>().Where(s => s.EquipmentId == equipmentId).ToListAsync(ct);
        return new Ctx { Equipment = equipment, Category = category, Setup = setup, Steps = steps };
    }

    private static List<EquipmentSetupStepDto> BuildSteps(Ctx c)
    {
        var done = c.Steps.Where(s => s.IsActive && s.Status == EquipmentSetupStepStatus.Complete).Select(s => s.StepNo).ToHashSet();
        if (c.Completed)
            for (var i = 1; i <= EquipmentSetupStepNo.Total; i++) done.Add(i);

        var list = new List<EquipmentSetupStepDto>();
        foreach (var d in StepDefs)
        {
            string status;
            if (done.Contains(d.No)) status = EquipmentSetupStepStatus.Complete;
            else if (d.No >= EquipmentSetupStepNo.Specifications && !done.Contains(d.No - 1)) status = EquipmentSetupStepStatus.Locked;
            else status = EquipmentSetupStepStatus.InProgress;
            list.Add(new EquipmentSetupStepDto(d.No, d.Key, d.Title, d.Hint, status));
        }
        return list;
    }

    private static string StepStatus(Ctx c, int stepNo) => BuildSteps(c).First(s => s.StepNo == stepNo).Status;

    private static void EnsureWritable(Ctx c, int stepNo)
    {
        if (c.Completed)
            throw new InvalidOperationException("The setup of this equipment is already completed.");
        if (StepStatus(c, stepNo) == EquipmentSetupStepStatus.Locked)
            throw new InvalidOperationException($"Step {stepNo} is locked until step {stepNo - 1} is complete.");
    }

    private static void StampNew(BaseEntity e, int userId, DateTime now)
    {
        e.UniqueID = Guid.NewGuid();
        e.CreatedBy = userId;
        e.CreatedDate = now;
        e.LastModifiedBy = userId;
        e.LastModifiedDate = now;
    }

    private static void StampChanged(BaseEntity e, int userId, DateTime now)
    {
        e.LastModifiedBy = userId;
        e.LastModifiedDate = now;
    }

    private EquipmentSetup EnsureSetup(Ctx c, int userId, DateTime now)
    {
        if (c.Setup is not null) return c.Setup;
        var setup = new EquipmentSetup { EquipmentId = c.Equipment.ID, Status = EquipmentSetupStatus.Draft, CurrentStep = 1, IsActive = true };
        StampNew(setup, userId, now);
        _db.Set<EquipmentSetup>().Add(setup);
        c.Setup = setup;
        return setup;
    }

    private EquipmentSetupStep EnsureStep(Ctx c, int stepNo, int userId, DateTime now)
    {
        var step = c.Steps.FirstOrDefault(s => s.StepNo == stepNo);
        if (step is not null) return step;
        step = new EquipmentSetupStep { EquipmentId = c.Equipment.ID, StepNo = stepNo, Status = EquipmentSetupStepStatus.InProgress, IsActive = true };
        StampNew(step, userId, now);
        _db.Set<EquipmentSetupStep>().Add(step);
        c.Steps.Add(step);
        return step;
    }

    /// <summary>"Save draft" keeps the step open; "Save &amp; continue" marks it complete and moves the wizard on.</summary>
    private async Task<EquipmentSetupStepResultDto> FinishStepAsync(Ctx c, int stepNo, bool draft, int userId, DateTime now, CancellationToken ct)
    {
        var setup = EnsureSetup(c, userId, now);
        var step = EnsureStep(c, stepNo, userId, now);

        if (!draft)
        {
            step.Status = EquipmentSetupStepStatus.Complete;
            step.CompletedAt ??= now;
            setup.CurrentStep = Math.Max(setup.CurrentStep, Math.Min(stepNo + 1, EquipmentSetupStepNo.Total));
        }
        StampChanged(step, userId, now);
        StampChanged(setup, userId, now);

        await _db.SaveChangesAsync(ct);
        return Result(c, stepNo);
    }

    private static EquipmentSetupStepResultDto Result(Ctx c, int stepNo)
    {
        var steps = BuildSteps(c);
        return new EquipmentSetupStepResultDto(
            stepNo,
            steps.First(s => s.StepNo == stepNo).Status,
            c.Setup?.Status ?? EquipmentSetupStatus.Draft,
            c.Setup?.CurrentStep ?? 1,
            steps);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    // ------------------------------------------------------------------------------------------------ overview + step 1

    public async Task<EquipmentSetupOverviewDto> GetOverviewAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        return new EquipmentSetupOverviewDto(
            c.Equipment.ID,
            c.Equipment.OBJECTID,
            c.Equipment.NAME,
            c.Setup?.Status ?? EquipmentSetupStatus.Draft,
            c.Setup?.CurrentStep ?? 1,
            EquipmentSetupStepNo.Total,
            BuildSteps(c));
    }

    public async Task<EquipmentDetailsStepDto> GetDetailsAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        return new EquipmentDetailsStepDto(DynamicsEquipmentMasterMapper.ToDto(c.Equipment), c.Category?.ID, c.Category?.NAME);
    }

    public async Task<EquipmentSetupStepResultDto> ConfirmDetailsAsync(int equipmentId, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.Details);
        return await FinishStepAsync(c, EquipmentSetupStepNo.Details, false, userId, DateTime.UtcNow, ct);
    }

    // ------------------------------------------------------------------------------------------------ step 2: information

    public async Task<EquipmentInformationDto> GetInformationAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        return ToInformation(c);
    }

    private static EquipmentInformationDto ToInformation(Ctx c) =>
        new(c.Equipment.ISPLPONUMBER, c.Equipment.VENDNAME, c.Equipment.ACQUISITIONCOST, c.Equipment.ACQUISITIONDATE,
            c.Category?.NAME ?? c.Equipment.OBJECTTYPE, c.Equipment.FUNCTIONALLOCATION);

    public async Task<EquipmentSetupStepResultDto> SaveInformationAsync(int equipmentId, EquipmentInformationRequest request, bool draft, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.Information);

        if (request.AcquisitionCost is < 0)
            throw new ArgumentException("Acquisition cost cannot be negative.");

        var now = DateTime.UtcNow;
        var e = c.Equipment;
        e.ISPLPONUMBER = Clean(request.PoNumber);
        e.VENDNAME = Clean(request.VendorName);
        e.ACQUISITIONCOST = request.AcquisitionCost;
        e.ACQUISITIONDATE = request.AcquisitionDate;
        StampChanged(e, userId, now);

        return await FinishStepAsync(c, EquipmentSetupStepNo.Information, draft, userId, now, ct);
    }

    // ------------------------------------------------------------------------------------------------ step 3: meter and fuel

    private static bool LogbookRequired(Ctx c) => c.Category?.ISLOGBOOKREQUIRED_ISPL == ISPLYesNo.Yes;
    private static bool FuelRequired(Ctx c) => c.Category?.ISFUELCONSUMPTIONREQUIRED_ISPL == ISPLYesNo.Yes;

    private async Task<EquipmentMeterEpochDetails?> LatestEpochAsync(int equipmentId, bool track, CancellationToken ct)
    {
        var query = _db.Set<EquipmentMeterEpochDetails>().Where(m => m.EquipmentID == equipmentId && m.IsActive);
        if (!track) query = query.AsNoTracking();
        return await query.OrderByDescending(m => m.ID).FirstOrDefaultAsync(ct);
    }

    public async Task<EquipmentMeterFuelDto> GetMeterFuelAsync(int equipmentId, CancellationToken ct)
    {
        var c = await LoadAsync(equipmentId, ct);
        var epoch = await LatestEpochAsync(equipmentId, false, ct);
        return new EquipmentMeterFuelDto(
            LogbookRequired(c), FuelRequired(c),
            epoch?.MeterReadingID, epoch?.OpenningDate, epoch?.OpeningReadingAmount,
            c.Equipment.STDFUELCONSUMPTION_ISPL);
    }

    public async Task<EquipmentSetupStepResultDto> SaveMeterFuelAsync(int equipmentId, EquipmentMeterFuelRequest request, bool draft, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.MeterFuel);
        var now = DateTime.UtcNow;

        // Only what the category asks for is saved; when both flags are off the step has nothing to enter.
        if (LogbookRequired(c))
        {
            if (!draft && (!request.ReadingTypeId.HasValue || !request.OpeningReading.HasValue || !request.OpeningDate.HasValue))
                throw new ArgumentException("Reading type, opening date and opening reading are required for this equipment category.");
            if (request.OpeningReading is < 0)
                throw new ArgumentException("Opening reading cannot be negative.");

            if (request.ReadingTypeId.HasValue)
            {
                var readingTypeId = request.ReadingTypeId.Value;
                var ok = await _db.Set<MeterReadingType>().AsNoTracking().AnyAsync(r => r.ID == readingTypeId && r.IsActive, ct);
                if (!ok) throw new ArgumentException("The selected reading type was not found or is not active.");
            }

            if (request.ReadingTypeId.HasValue || request.OpeningReading.HasValue || request.OpeningDate.HasValue)
            {
                var epoch = await LatestEpochAsync(equipmentId, true, ct);
                if (epoch is null)
                {
                    epoch = new EquipmentMeterEpochDetails { EquipmentID = equipmentId, IsActive = true };
                    StampNew(epoch, userId, now);
                    _db.Set<EquipmentMeterEpochDetails>().Add(epoch);
                }
                else
                {
                    StampChanged(epoch, userId, now);
                }
                epoch.MeterReadingID = request.ReadingTypeId;
                epoch.OpeningReadingAmount = request.OpeningReading;
                epoch.OpenningDate = request.OpeningDate;
            }
        }

        if (FuelRequired(c))
        {
            if (!draft && !request.FuelStandard.HasValue)
                throw new ArgumentException("Fuel standard is required for this equipment category.");
            if (request.FuelStandard is < 0)
                throw new ArgumentException("Fuel standard cannot be negative.");
            if (request.FuelStandard.HasValue)
            {
                c.Equipment.STDFUELCONSUMPTION_ISPL = request.FuelStandard;
                StampChanged(c.Equipment, userId, now);
            }
        }

        return await FinishStepAsync(c, EquipmentSetupStepNo.MeterFuel, draft, userId, now, ct);
    }

    // ------------------------------------------------------------------------------------------------ step 4: documents

    public async Task<EquipmentDocumentsStepDto> GetDocumentsAsync(int equipmentId, CancellationToken ct)
    {
        await LoadAsync(equipmentId, ct);
        var docs = await _db.Set<EquipmentSetupDocument>().AsNoTracking()
            .Where(d => d.EquipmentId == equipmentId && d.IsActive).OrderBy(d => d.ID).ToListAsync(ct);
        var typeIds = docs.Select(d => d.DocumentTypeId).Distinct().ToList();
        var types = await _db.Set<DocumentType>().AsNoTracking().Where(t => typeIds.Contains(t.ID)).ToListAsync(ct);
        var typeNames = types.ToDictionary(t => t.ID, t => t.DocumentTypeName);

        var items = docs.Select(d => new EquipmentDocumentDto(
            d.ID, d.DocumentTypeId, typeNames.TryGetValue(d.DocumentTypeId, out var n) ? n : string.Empty,
            d.DocumentName, d.StartDate, d.EndDate, d.StoredFileName, d.OriginalFileName, d.Remarks)).ToList();
        return new EquipmentDocumentsStepDto(items.Count, items.Count(i => !string.IsNullOrEmpty(i.StoredFileName)), items);
    }

    public async Task<EquipmentSetupStepResultDto> SaveDocumentsAsync(int equipmentId, EquipmentDocumentsSaveRequest request, bool draft, CancellationToken ct)
    {
        var userId = CurrentUserId;
        var c = await LoadAsync(equipmentId, ct);
        EnsureWritable(c, EquipmentSetupStepNo.Documents);
        var items = request.Items ?? new List<EquipmentDocumentRequest>();

        var typeIds = items.Select(i => i.DocumentTypeId).Distinct().ToList();
        var activeTypes = (await _db.Set<DocumentType>().AsNoTracking().Where(t => typeIds.Contains(t.ID) && t.IsActive).ToListAsync(ct))
            .Select(t => t.ID).ToHashSet();

        foreach (var i in items)
        {
            if (!activeTypes.Contains(i.DocumentTypeId))
                throw new ArgumentException($"Document type {i.DocumentTypeId} was not found or is not active.");
            if (string.IsNullOrWhiteSpace(i.DocumentName))
                throw new ArgumentException("Document name is required.");
            if (i.DocumentName.Trim().Length > 200)
                throw new ArgumentException("Document name can be at most 200 characters.");
            if (i.StartDate.HasValue && i.EndDate.HasValue && i.EndDate.Value < i.StartDate.Value)
                throw new ArgumentException($"End date of '{i.DocumentName.Trim()}' cannot be before its start date.");
            if (i.StoredFileName is { Length: > 300 } || i.OriginalFileName is { Length: > 300 })
                throw new ArgumentException("File names can be at most 300 characters.");
            if (i.Remarks is { Length: > 500 })
                throw new ArgumentException("Remarks can be at most 500 characters.");
        }

        var existing = await _db.Set<EquipmentSetupDocument>().Where(d => d.EquipmentId == equipmentId).ToListAsync(ct);
        var byId = existing.ToDictionary(d => d.ID);
        var now = DateTime.UtcNow;
        var keptIds = new HashSet<int>();

        foreach (var i in items)
        {
            EquipmentSetupDocument doc;
            if (i.Id.HasValue && i.Id.Value > 0)
            {
                if (!byId.TryGetValue(i.Id.Value, out doc!))
                    throw new ArgumentException($"Document {i.Id.Value} does not belong to this equipment.");
                if (!keptIds.Add(doc.ID))
                    throw new ArgumentException($"Document {doc.ID} is listed more than once.");
                StampChanged(doc, userId, now);
            }
            else
            {
                doc = new EquipmentSetupDocument { EquipmentId = equipmentId };
                StampNew(doc, userId, now);
                _db.Set<EquipmentSetupDocument>().Add(doc);
            }

            doc.IsActive = true;
            doc.DocumentTypeId = i.DocumentTypeId;
            doc.DocumentName = i.DocumentName.Trim();
            doc.StartDate = i.StartDate;
            doc.EndDate = i.EndDate;
            doc.StoredFileName = Clean(i.StoredFileName);
            doc.OriginalFileName = Clean(i.OriginalFileName);
            doc.Remarks = Clean(i.Remarks);
        }

        // Documents that are no longer in the list are switched off (kept for history).
        foreach (var doc in existing.Where(d => d.IsActive && !keptIds.Contains(d.ID) && !(items.Any(x => x.Id == d.ID))))
        {
            doc.IsActive = false;
            StampChanged(doc, userId, now);
        }

        return await FinishStepAsync(c, EquipmentSetupStepNo.Documents, draft, userId, now, ct);
    }
}
