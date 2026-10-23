using Himapp.PM.Application.Features.EquipmentCategoryLinks;
using Himapp.PM.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Services;

/// <summary>
/// Manual mapping between an equipment category and its six masters
/// (sub-parts, work descriptions, breakdown reasons, PM points, checklist points, specifications).
/// Errors: KeyNotFoundException = not found (404), ArgumentException = bad input (400),
/// InvalidOperationException = already linked (409), UnauthorizedAccessException = no user in the token (401).
/// </summary>
public interface IEquipmentCategoryLinkService
{
    bool IsKnownKind(string kind);
    Task<EquipmentCategoryConfigurationDto> GetConfigurationAsync(int categoryId, CancellationToken ct);
    Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> GetAsync(int categoryId, string kind, bool includeInactive, CancellationToken ct);
    Task<IReadOnlyList<EquipmentCategoryLinkOptionDto>> GetAvailableAsync(int categoryId, string kind, CancellationToken ct);
    Task<EquipmentCategoryLinkItemDto> AddAsync(int categoryId, string kind, EquipmentCategoryLinkRequest request, CancellationToken ct);
    Task<EquipmentCategoryLinkItemDto?> UpdateAsync(int categoryId, string kind, int linkId, EquipmentCategoryLinkUpdateRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int categoryId, string kind, int linkId, CancellationToken ct);
    Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> SaveAllAsync(int categoryId, string kind, EquipmentCategoryLinkSaveRequest request, CancellationToken ct);
}

public sealed class EquipmentCategoryLinkService : IEquipmentCategoryLinkService
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly Dictionary<string, ILinkKind> _kinds;

    public EquipmentCategoryLinkService(IPMDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;

        // Order here is the order of the "Configuration points" panel.
        var list = new List<ILinkKind>
        {
            new LinkKind<EquipmentCategorySubPart, EquipmentSubPart>(db, "sub-parts", "Equipment Sub-Part", "sub-part",
                l => l.EquipmentSubPartId, (l, id) => l.EquipmentSubPartId = id, m => m.EquipmentSubPartName, m => m.IsActive),
            new LinkKind<EquipmentCategoryWorkDescription, WorkDescription>(db, "work-descriptions", "Work Descriptions", "work description",
                l => l.WorkDescriptionId, (l, id) => l.WorkDescriptionId = id, m => m.WorkDescriptionName, m => m.IsActive),
            new LinkKind<EquipmentCategoryBreakdownReason, EquipmentBreakdownReasons>(db, "breakdown-reasons", "Breakdown Reasons", "breakdown reason",
                l => l.EquipmentBreakdownReasonId, (l, id) => l.EquipmentBreakdownReasonId = id, m => m.BreakdownReason, m => m.IsActive),
            new LinkKind<EquipmentCategoryPMPoint, PreventiveMaintenancePoint>(db, "pm-points", "PM Points", "PM point",
                l => l.PreventiveMaintenancePointId, (l, id) => l.PreventiveMaintenancePointId = id, m => m.MaintenancePointName ?? string.Empty, m => m.IsActive,
                readTrigger: l => new EquipmentCategoryPmTrigger
                {
                    ReadingTypeId = l.ReadingTypeId,
                    IntervalValue = l.IntervalValue,
                    ToleranceValue = l.ToleranceValue,
                    Months = l.Months,
                    MonthsTolerance = l.MonthsTolerance
                },
                writeTrigger: (l, t) =>
                {
                    l.ReadingTypeId = t.ReadingTypeId;
                    l.IntervalValue = t.IntervalValue;
                    l.ToleranceValue = t.ToleranceValue;
                    l.Months = t.Months;
                    l.MonthsTolerance = t.MonthsTolerance;
                },
                validateTrigger: async (t, ct) =>
                {
                    if (t.ReadingTypeId.HasValue != t.IntervalValue.HasValue)
                        throw new ArgumentException("ReadingTypeId and IntervalValue must be given together.");
                    if (t.IntervalValue.HasValue && t.IntervalValue.Value <= 0)
                        throw new ArgumentException("IntervalValue must be greater than 0.");
                    if (t.Months.HasValue && t.Months.Value <= 0)
                        throw new ArgumentException("Months must be greater than 0.");
                    if ((t.ToleranceValue.HasValue && (t.ToleranceValue.Value < 0 || t.ToleranceValue.Value > 100)) ||
                        (t.MonthsTolerance.HasValue && (t.MonthsTolerance.Value < 0 || t.MonthsTolerance.Value > 100)))
                        throw new ArgumentException("Tolerance must be between 0 and 100 (percent).");
                    if (t.ReadingTypeId.HasValue)
                    {
                        var readingTypeId = t.ReadingTypeId.Value;
                        var ok = await db.Set<MeterReadingType>().AsNoTracking().AnyAsync(r => r.ID == readingTypeId && r.IsActive, ct);
                        if (!ok) throw new ArgumentException("The selected reading type was not found or is not active.");
                    }
                }),
            new LinkKind<EquipmentCategoryChecklistPoint, EquipmentChecklistPoint>(db, "checklist-points", "Equipment Checklist Points", "checklist point",
                l => l.EquipmentChecklistPointId, (l, id) => l.EquipmentChecklistPointId = id, m => m.Description, m => m.IsActive),
            new LinkKind<EquipmentCategorySpecification, SpecificationMaster>(db, "specifications", "Specifications", "specification",
                l => l.SpecificationMasterId, (l, id) => l.SpecificationMasterId = id, m => m.SpecificationName, m => m.IsActive),
        };
        _kinds = list.ToDictionary(k => k.Slug, StringComparer.OrdinalIgnoreCase);
    }

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public bool IsKnownKind(string kind) => _kinds.ContainsKey(kind);

    private ILinkKind Kind(string kind) =>
        _kinds.TryGetValue(kind, out var k) ? k : throw new KeyNotFoundException($"Unknown configuration point '{kind}'.");

    private async Task EnsureCategoryAsync(int categoryId, CancellationToken ct)
    {
        var exists = await _db.Set<DynamicsEquipmentCategory>().AsNoTracking().AnyAsync(c => c.ID == categoryId, ct);
        if (!exists) throw new KeyNotFoundException($"Equipment category {categoryId} was not found.");
    }

    public async Task<EquipmentCategoryConfigurationDto> GetConfigurationAsync(int categoryId, CancellationToken ct)
    {
        await EnsureCategoryAsync(categoryId, ct);
        var points = new List<EquipmentCategoryConfigurationPointDto>();
        foreach (var k in _kinds.Values)
        {
            var count = await k.CountActiveAsync(categoryId, ct);
            points.Add(new EquipmentCategoryConfigurationPointDto(k.Slug, k.Label, count, count > 0));
        }
        return new EquipmentCategoryConfigurationDto(categoryId, points.Count(p => p.IsConfigured), points.Count, points);
    }

    public async Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> GetAsync(int categoryId, string kind, bool includeInactive, CancellationToken ct)
    {
        var k = Kind(kind);
        await EnsureCategoryAsync(categoryId, ct);
        return await k.ListAsync(categoryId, includeInactive, ct);
    }

    public async Task<IReadOnlyList<EquipmentCategoryLinkOptionDto>> GetAvailableAsync(int categoryId, string kind, CancellationToken ct)
    {
        var k = Kind(kind);
        await EnsureCategoryAsync(categoryId, ct);
        return await k.AvailableAsync(categoryId, ct);
    }

    public async Task<EquipmentCategoryLinkItemDto> AddAsync(int categoryId, string kind, EquipmentCategoryLinkRequest request, CancellationToken ct)
    {
        var k = Kind(kind);
        var userId = CurrentUserId;
        await EnsureCategoryAsync(categoryId, ct);
        return await k.AddAsync(categoryId, request, userId, ct);
    }

    public async Task<EquipmentCategoryLinkItemDto?> UpdateAsync(int categoryId, string kind, int linkId, EquipmentCategoryLinkUpdateRequest request, CancellationToken ct)
    {
        var k = Kind(kind);
        var userId = CurrentUserId;
        return await k.UpdateAsync(categoryId, linkId, request, userId, ct);
    }

    public async Task<bool> DeleteAsync(int categoryId, string kind, int linkId, CancellationToken ct)
    {
        var k = Kind(kind);
        var userId = CurrentUserId;
        return await k.RemoveAsync(categoryId, linkId, userId, ct);
    }

    public async Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> SaveAllAsync(int categoryId, string kind, EquipmentCategoryLinkSaveRequest request, CancellationToken ct)
    {
        var k = Kind(kind);
        var userId = CurrentUserId;
        await EnsureCategoryAsync(categoryId, ct);
        return await k.SaveAllAsync(categoryId, request.Items ?? new List<EquipmentCategoryLinkRequest>(), userId, ct);
    }

    // ------------------------------------------------------------------------------------------------

    private interface ILinkKind
    {
        string Slug { get; }
        string Label { get; }
        Task<int> CountActiveAsync(int categoryId, CancellationToken ct);
        Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> ListAsync(int categoryId, bool includeInactive, CancellationToken ct);
        Task<IReadOnlyList<EquipmentCategoryLinkOptionDto>> AvailableAsync(int categoryId, CancellationToken ct);
        Task<EquipmentCategoryLinkItemDto> AddAsync(int categoryId, EquipmentCategoryLinkRequest request, int userId, CancellationToken ct);
        Task<EquipmentCategoryLinkItemDto?> UpdateAsync(int categoryId, int linkId, EquipmentCategoryLinkUpdateRequest request, int userId, CancellationToken ct);
        Task<bool> RemoveAsync(int categoryId, int linkId, int userId, CancellationToken ct);
        Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> SaveAllAsync(int categoryId, List<EquipmentCategoryLinkRequest> items, int userId, CancellationToken ct);
    }

    /// <summary>The same rules for all six links; only the master table and the master column differ.</summary>
    private sealed class LinkKind<TLink, TMaster> : ILinkKind
        where TLink : EquipmentCategoryLinkBase, new()
        where TMaster : BaseEntity
    {
        private readonly IPMDbContext _db;
        private readonly string _noun;
        private readonly Func<TLink, int> _getMasterId;
        private readonly Action<TLink, int> _setMasterId;
        private readonly Func<TMaster, string> _getName;
        private readonly Func<TMaster, bool> _isActive;
        private readonly Func<TLink, EquipmentCategoryPmTrigger?>? _readTrigger;
        private readonly Action<TLink, EquipmentCategoryPmTrigger>? _writeTrigger;
        private readonly Func<EquipmentCategoryPmTrigger, CancellationToken, Task>? _validateTrigger;

        public string Slug { get; }
        public string Label { get; }

        public LinkKind(IPMDbContext db, string slug, string label, string noun,
            Func<TLink, int> getMasterId, Action<TLink, int> setMasterId, Func<TMaster, string> getName, Func<TMaster, bool> isActive,
            Func<TLink, EquipmentCategoryPmTrigger?>? readTrigger = null,
            Action<TLink, EquipmentCategoryPmTrigger>? writeTrigger = null,
            Func<EquipmentCategoryPmTrigger, CancellationToken, Task>? validateTrigger = null)
        {
            _db = db; Slug = slug; Label = label; _noun = noun;
            _getMasterId = getMasterId; _setMasterId = setMasterId; _getName = getName; _isActive = isActive;
            _readTrigger = readTrigger; _writeTrigger = writeTrigger; _validateTrigger = validateTrigger;
        }

        private Task ValidateTriggerAsync(EquipmentCategoryPmTrigger? trigger, CancellationToken ct) =>
            trigger is not null && _validateTrigger is not null ? _validateTrigger(trigger, ct) : Task.CompletedTask;

        private void ApplyTrigger(TLink link, EquipmentCategoryPmTrigger? trigger)
        {
            if (trigger is not null && _writeTrigger is not null) _writeTrigger(link, trigger);
        }

        public Task<int> CountActiveAsync(int categoryId, CancellationToken ct) =>
            _db.Set<TLink>().AsNoTracking().CountAsync(l => l.EquipmentCategoryId == categoryId && l.IsActive, ct);

        public async Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> ListAsync(int categoryId, bool includeInactive, CancellationToken ct)
        {
            var links = await _db.Set<TLink>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && (includeInactive || l.IsActive))
                .OrderBy(l => l.ID)
                .ToListAsync(ct);
            return await ToDtosAsync(links, ct);
        }

        public async Task<IReadOnlyList<EquipmentCategoryLinkOptionDto>> AvailableAsync(int categoryId, CancellationToken ct)
        {
            var linkedIds = (await _db.Set<TLink>().AsNoTracking()
                .Where(l => l.EquipmentCategoryId == categoryId && l.IsActive)
                .ToListAsync(ct)).Select(_getMasterId).ToHashSet();

            var masters = await _db.Set<TMaster>().AsNoTracking().OrderBy(m => m.ID).ToListAsync(ct);
            return masters
                .Where(m => _isActive(m) && !linkedIds.Contains(m.ID))
                .Select(m => new EquipmentCategoryLinkOptionDto(m.ID, _getName(m)))
                .OrderBy(o => o.MasterName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<EquipmentCategoryLinkItemDto> AddAsync(int categoryId, EquipmentCategoryLinkRequest request, int userId, CancellationToken ct)
        {
            var master = await _db.Set<TMaster>().AsNoTracking().FirstOrDefaultAsync(m => m.ID == request.MasterId, ct);
            if (master is null || !_isActive(master))
                throw new ArgumentException($"The selected {_noun} was not found or is not active.");
            await ValidateTriggerAsync(request.PmTrigger, ct);

            var links = await _db.Set<TLink>().Where(l => l.EquipmentCategoryId == categoryId).ToListAsync(ct);
            var link = links.FirstOrDefault(l => _getMasterId(l) == request.MasterId);
            var now = DateTime.UtcNow;

            if (link is not null)
            {
                if (link.IsActive)
                    throw new InvalidOperationException($"This {_noun} is already linked to the equipment category.");

                link.IsActive = true;
                link.Remarks = Clean(request.Remarks);
                ApplyTrigger(link, request.PmTrigger);
                Touch(link, userId, now);
            }
            else
            {
                link = NewLink(categoryId, request.MasterId, request.Remarks, userId, now);
                ApplyTrigger(link, request.PmTrigger);
                _db.Set<TLink>().Add(link);
            }

            await _db.SaveChangesAsync(ct);
            return ToDto(link, _getName(master));
        }

        public async Task<EquipmentCategoryLinkItemDto?> UpdateAsync(int categoryId, int linkId, EquipmentCategoryLinkUpdateRequest request, int userId, CancellationToken ct)
        {
            var link = await _db.Set<TLink>().FirstOrDefaultAsync(l => l.ID == linkId && l.EquipmentCategoryId == categoryId, ct);
            if (link is null) return null;

            await ValidateTriggerAsync(request.PmTrigger, ct);
            link.Remarks = Clean(request.Remarks);
            ApplyTrigger(link, request.PmTrigger);
            if (request.IsActive.HasValue) link.IsActive = request.IsActive.Value;
            Touch(link, userId, DateTime.UtcNow);
            await _db.SaveChangesAsync(ct);

            return (await ToDtosAsync(new List<TLink> { link }, ct))[0];
        }

        public async Task<bool> RemoveAsync(int categoryId, int linkId, int userId, CancellationToken ct)
        {
            var link = await _db.Set<TLink>().FirstOrDefaultAsync(l => l.ID == linkId && l.EquipmentCategoryId == categoryId, ct);
            if (link is null) return false;

            if (link.IsActive)
            {
                link.IsActive = false;
                Touch(link, userId, DateTime.UtcNow);
                await _db.SaveChangesAsync(ct);
            }
            return true;
        }

        public async Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> SaveAllAsync(int categoryId, List<EquipmentCategoryLinkRequest> items, int userId, CancellationToken ct)
        {
            var duplicate = items.GroupBy(i => i.MasterId).FirstOrDefault(g => g.Count() > 1);
            if (duplicate is not null)
                throw new ArgumentException($"The same {_noun} (id {duplicate.Key}) is listed more than once.");

            foreach (var item in items) await ValidateTriggerAsync(item.PmTrigger, ct);

            var wantedIds = items.Select(i => i.MasterId).ToList();
            var masters = await _db.Set<TMaster>().AsNoTracking().Where(m => wantedIds.Contains(m.ID)).ToListAsync(ct);
            var masterById = masters.ToDictionary(m => m.ID);

            var links = await _db.Set<TLink>().Where(l => l.EquipmentCategoryId == categoryId).ToListAsync(ct);
            var linkByMaster = links.ToDictionary(_getMasterId);
            var now = DateTime.UtcNow;

            foreach (var item in items)
            {
                if (!masterById.TryGetValue(item.MasterId, out var master))
                    throw new ArgumentException($"The selected {_noun} (id {item.MasterId}) was not found.");

                var remarks = Clean(item.Remarks);
                if (linkByMaster.TryGetValue(item.MasterId, out var link))
                {
                    if (link.IsActive)
                    {
                        // Already linked: only remarks may change; a master switched off later does not block saving the tab.
                        if (link.Remarks != remarks || item.PmTrigger is not null)
                        {
                            link.Remarks = remarks;
                            ApplyTrigger(link, item.PmTrigger);
                            Touch(link, userId, now);
                        }
                        continue;
                    }

                    if (!_isActive(master))
                        throw new ArgumentException($"The selected {_noun} '{_getName(master)}' is not active.");
                    link.IsActive = true;
                    link.Remarks = remarks;
                    ApplyTrigger(link, item.PmTrigger);
                    Touch(link, userId, now);
                }
                else
                {
                    if (!_isActive(master))
                        throw new ArgumentException($"The selected {_noun} '{_getName(master)}' is not active.");
                    var created = NewLink(categoryId, item.MasterId, remarks, userId, now);
                    ApplyTrigger(created, item.PmTrigger);
                    _db.Set<TLink>().Add(created);
                }
            }

            // Anything that was linked before but is no longer in the list is switched off (kept for history).
            var keep = wantedIds.ToHashSet();
            foreach (var link in links.Where(l => l.IsActive && !keep.Contains(_getMasterId(l))))
            {
                link.IsActive = false;
                Touch(link, userId, now);
            }

            await _db.SaveChangesAsync(ct);
            return await ListAsync(categoryId, false, ct);
        }

        // --------------------------------------------------------------------------------------------

        private TLink NewLink(int categoryId, int masterId, string? remarks, int userId, DateTime now)
        {
            var link = new TLink
            {
                UniqueID = Guid.NewGuid(),
                EquipmentCategoryId = categoryId,
                Remarks = Clean(remarks),
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = now,
                LastModifiedBy = userId,
                LastModifiedDate = now
            };
            _setMasterId(link, masterId);
            return link;
        }

        private static void Touch(TLink link, int userId, DateTime now)
        {
            link.LastModifiedBy = userId;
            link.LastModifiedDate = now;
        }

        private static string? Clean(string? remarks) => string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();

        private async Task<IReadOnlyList<EquipmentCategoryLinkItemDto>> ToDtosAsync(List<TLink> links, CancellationToken ct)
        {
            var ids = links.Select(_getMasterId).Distinct().ToList();
            var masters = await _db.Set<TMaster>().AsNoTracking().Where(m => ids.Contains(m.ID)).ToListAsync(ct);
            var names = masters.ToDictionary(m => m.ID, m => _getName(m));
            return links.Select(l => ToDto(l, names.TryGetValue(_getMasterId(l), out var n) ? n : string.Empty)).ToList();
        }

        private EquipmentCategoryLinkItemDto ToDto(TLink l, string masterName) =>
            new(l.ID, _getMasterId(l), masterName, l.Remarks, l.IsActive, l.CreatedBy, l.CreatedDate, l.LastModifiedBy, l.LastModifiedDate, _readTrigger?.Invoke(l));
    }
}
