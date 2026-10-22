using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.Equipments.Commands;
using Himapp.PM.Application.Features.Equipments.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.Equipments.Handlers;

internal sealed class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, EquipmentDto>
{
    private readonly IEquipmentRepository _repository;
    public CreateEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.AddAsync(new EquipmentDto(0, request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn, "Draft"), cancellationToken);
}

internal sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, EquipmentDto?>
{
    private readonly IEquipmentRepository _repository;
    public UpdateEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto?> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.UpdateAsync(new EquipmentDto(request.Id, request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn, "Available"), cancellationToken);
}

internal sealed class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _repository;
    public DeleteEquipmentCommandHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllEquipmentsQueryHandler : IRequestHandler<GetAllEquipmentsQuery, IReadOnlyCollection<EquipmentDto>>
{
    private readonly IEquipmentRepository _repository;
    public GetAllEquipmentsQueryHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<IReadOnlyCollection<EquipmentDto>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken) =>
        _repository.GetAllAsync(cancellationToken);
}

internal sealed class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    private readonly IEquipmentRepository _repository;
    public GetEquipmentByIdQueryHandler(IEquipmentRepository repository) => _repository = repository;
    public Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.Id, cancellationToken);
}

public sealed class GetMeterEpochRegisterQueryHandler : IRequestHandler<GetMeterEpochRegisterQuery, IReadOnlyList<MeterEpochRegisterRow>>
{
    private readonly IPMDbContext _db;

    public GetMeterEpochRegisterQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyList<MeterEpochRegisterRow>> Handle(GetMeterEpochRegisterQuery request, CancellationToken ct)
    {
        // 1. Load epochs — scalar, no nav
        var epochs = await _db.Set<EquipmentMeterEpochDetails>()
            .AsNoTracking()
            .Where(e => e.EquipmentID == request.EquipmentId)
            .OrderByDescending(e => e.OpenningDate)
            .ToListAsync(ct);

        if (epochs.Count == 0) return Array.Empty<MeterEpochRegisterRow>();

        // 2. Load only the ReasonIDs used — batch lookup, no nav
        var reasonIds = epochs
            .Where(e => e.ReasonID.HasValue)
            .Select(e => e.ReasonID!.Value)
            .Distinct()
            .ToList();

        // 3. Compute closing date from the next-newer epoch
        var rows = new List<MeterEpochRegisterRow>(epochs.Count);
        for (var i = 0; i < epochs.Count; i++)
        {
            var current = epochs[i];
            var newer = i > 0 ? epochs[i - 1] : null;   // ordered DESC

            rows.Add(new MeterEpochRegisterRow(
                EpochID: current.ID,
                OpenningDate: DateOnly.FromDateTime(current.OpenningDate!.Value),
                ClosingDate: newer?.OpenningDate is { } c
                                           ? DateOnly.FromDateTime(c.AddDays(-1))
                                           : null,
                ResetReadingAmount: current.ResetReadingAmount,
                OpeningReadingAmount: current.OpeningReadingAmount,
                ReasonID: current.ReasonID,
                ReasonName: null,               // fill from lookup if needed
                EpochUsage: (current.ResetReadingAmount ?? 0)
                                     - (current.OpeningReadingAmount ?? 0),
                IsActive: current.IsActive,
                Remarks: current.Remarks));
        }

        return rows;
    }

    public sealed class GetAwaitingPMConfigurationQueueQueryHandler : IRequestHandler<GetAwaitingPMConfigurationQueueQuery, PagedResult<AwaitingPMConfigurationRow>>
    {
        private readonly IPMDbContext _db;

        public GetAwaitingPMConfigurationQueueQueryHandler(IPMDbContext db) => _db = db;

        public async Task<PagedResult<AwaitingPMConfigurationRow>> Handle(GetAwaitingPMConfigurationQueueQuery request, CancellationToken ct)
        {
            var configuredEquipmentIds = _db.Set<EquipmentPreventiveMaintenancePoints>()
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => p.EquipmentID);

            // Assumes DynamicsEquipmentMaster is the equipment source.
            // No nav to EquipmentPreventiveMaintenancePoints — plain subquery.
            var q = _db.Set<DynamicsEquipmentMaster>()
                .AsNoTracking()
                .Where(e => !configuredEquipmentIds.Contains(e.ID));

            if (request.CategoryId is int cat)
                q = q.Where(e => e.CATEGORYLEVEL1 == cat.ToString()
                              || e.CATEGORYLEVEL2 == cat.ToString()
                              || e.CATEGORYLEVEL3 == cat.ToString());

            if (!string.IsNullOrWhiteSpace(request.Search))
                q = q.Where(e => e.NAME.Contains(request.Search));

            var total = await q.CountAsync(ct);
            var rows = await q
                .OrderBy(e => e.NAME)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new AwaitingPMConfigurationRow(
                    EquipmentID: e.ID,
                    AssetCode: e.ISPLITEMID,
                    Name: e.NAME,
                    Category: e.CATEGORYLEVEL1,
                    DefaultPMCount: null,  // unavailable until category defaults are defined
                    ConfiguredPMCount: _db.Set<EquipmentPreventiveMaintenancePoints>()
                        .Count(p => p.EquipmentID == e.ID && p.IsActive),
                    OnboardedOn: e.CreatedAt))
                .ToListAsync(ct);

            return new PagedResult<AwaitingPMConfigurationRow>(rows, total, request.Page, request.PageSize);
        }
    }
}


internal static class EquipmentPMConfigurationMapper
{
    public static EquipmentPMConfigurationResponse Map(EquipmentPreventiveMaintenancePoints e) =>
        new(e.ID, e.UniqueID, e.EquipmentID ?? 0, e.MaintenancePointID, e.ItemID, e.ReadingTypeID,
            e.IntervalValue, e.TolerenceValue, e.Months, e.Remarks, e.StatusID, e.IsActive,
            e.CreatedAt, e.ModifiedAt);
}

internal sealed class CreateEquipmentPMConfigurationCommandHandler : IRequestHandler<CreateEquipmentPMConfigurationCommand, EquipmentPMConfigurationResponse>
{
    private readonly IPMDbContext _db;
    public CreateEquipmentPMConfigurationCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentPMConfigurationResponse> Handle(CreateEquipmentPMConfigurationCommand request, CancellationToken ct)
    {
        var req = request.Request;

        if (req.MaintenancePointID is null)
            throw new InvalidOperationException("MaintenancePointID is required.");

        if (!req.IntervalValue.HasValue && !req.Months.HasValue)
            throw new InvalidOperationException("At least one of IntervalValue or Months must be provided.");

        var equipmentExists = await _db.Set<DynamicsEquipmentMaster>()
            .AnyAsync(x => x.ID == request.EquipmentId, ct);
        if (!equipmentExists)
            throw new InvalidOperationException($"Equipment {request.EquipmentId} not found.");

        var mpExists = await _db.Set<PreventiveMaintenancePoint>()
            .AnyAsync(x => x.ID == req.MaintenancePointID && x.IsActive, ct);
        if (!mpExists)
            throw new InvalidOperationException($"Maintenance point {req.MaintenancePointID} not found or inactive.");

        if (req.ReadingTypeID is int rt)
        {
            var rtExists = await _db.Set<MeterReadingType>().AnyAsync(x => x.ID == rt && x.IsActive, ct);
            if (!rtExists)
                throw new InvalidOperationException($"Reading type {rt} not found or inactive.");
        }

        var duplicate = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AnyAsync(x => x.EquipmentID == request.EquipmentId
                        && x.MaintenancePointID == req.MaintenancePointID, ct);
        if (duplicate)
            throw new InvalidOperationException(
                $"Maintenance point {req.MaintenancePointID} is already configured for equipment {request.EquipmentId}.");

        var entity = new EquipmentPreventiveMaintenancePoints
        {
            EquipmentID = request.EquipmentId,
            MaintenancePointID = req.MaintenancePointID,
            ItemID = req.ItemID,
            ReadingTypeID = req.ReadingTypeID,
            IntervalValue = req.IntervalValue,
            TolerenceValue = req.TolerenceValue,
            Months = req.Months,
            Remarks = req.Remarks,
            StatusID = req.StatusID,
            IsActive = true
        };

        _db.Set<EquipmentPreventiveMaintenancePoints>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return EquipmentPMConfigurationMapper.Map(entity);
    }
}

internal sealed class UpdateEquipmentPMConfigurationCommandHandler : IRequestHandler<UpdateEquipmentPMConfigurationCommand, EquipmentPMConfigurationResponse?>
{
    private readonly IPMDbContext _db;
    public UpdateEquipmentPMConfigurationCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentPMConfigurationResponse?> Handle(UpdateEquipmentPMConfigurationCommand request, CancellationToken ct)
    {
        var entity = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.EquipmentID == request.EquipmentId, ct);
        if (entity is null) return null;

        var req = request.Request;
        if (!req.IntervalValue.HasValue && !req.Months.HasValue)
            throw new InvalidOperationException("At least one of IntervalValue or Months must be provided.");

        entity.MaintenancePointID = req.MaintenancePointID ?? entity.MaintenancePointID;
        entity.ItemID = req.ItemID;
        entity.ReadingTypeID = req.ReadingTypeID;
        entity.IntervalValue = req.IntervalValue;
        entity.TolerenceValue = req.TolerenceValue;
        entity.Months = req.Months;
        entity.Remarks = req.Remarks;
        entity.StatusID = req.StatusID;

        await _db.SaveChangesAsync(ct);
        return EquipmentPMConfigurationMapper.Map(entity);
    }
}

internal sealed class GetEquipmentPMConfigurationsByEquipmentQueryHandler : IRequestHandler<GetEquipmentPMConfigurationsByEquipmentQuery, IReadOnlyList<EquipmentPMConfigurationResponse>>
{
    private readonly IPMDbContext _db;
    public GetEquipmentPMConfigurationsByEquipmentQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyList<EquipmentPMConfigurationResponse>> Handle(GetEquipmentPMConfigurationsByEquipmentQuery request, CancellationToken ct)
    {
        return await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AsNoTracking()
            .Where(x => x.EquipmentID == request.EquipmentId)
            .OrderBy(x => x.MaintenancePointID)
            .Select(x => new EquipmentPMConfigurationResponse(
                x.ID, x.UniqueID, x.EquipmentID ?? 0, x.MaintenancePointID, x.ItemID, x.ReadingTypeID,
                x.IntervalValue, x.TolerenceValue, x.Months, x.Remarks, x.StatusID, x.IsActive,
                x.CreatedAt, x.ModifiedAt))
            .ToListAsync(ct);
    }
}

internal sealed class GetEquipmentPMConfigurationByIdQueryHandler : IRequestHandler<GetEquipmentPMConfigurationByIdQuery, EquipmentPMConfigurationResponse?>
{
    private readonly IPMDbContext _db;
    public GetEquipmentPMConfigurationByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentPMConfigurationResponse?> Handle(
        GetEquipmentPMConfigurationByIdQuery request, CancellationToken ct)
    {
        var e = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.EquipmentID == request.EquipmentId, ct);
        return e is null ? null : EquipmentPMConfigurationMapper.Map(e);
    }
}