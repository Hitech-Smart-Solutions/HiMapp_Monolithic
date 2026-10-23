using Himapp.PM.Application.Features.Equipments.Models;
using Himapp.PM.Application.Features.Equipments.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.Equipments.Handlers;

public sealed class GetEquipmentMeterRegisterSummaryQueryHandler
    : IRequestHandler<GetEquipmentMeterRegisterSummaryQuery, EquipmentMeterRegisterDto?>
{
    private readonly IPMDbContext _db;

    public GetEquipmentMeterRegisterSummaryQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentMeterRegisterDto?> Handle(
        GetEquipmentMeterRegisterSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var equipmentExists = await _db.Set<DynamicsEquipmentMaster>()
            .AsNoTracking()
            .AnyAsync(equipment => equipment.ID == request.EquipmentId, cancellationToken);

        if (!equipmentExists)
            return null;

        var cumulativeRecord = await _db.Set<EquipmentMeterCumulativeDetail>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                record => record.EquipmentID == request.EquipmentId,
                cancellationToken);

        if (cumulativeRecord is null)
            return null;

        var epochs = await _db.Set<EquipmentMeterEpochDetails>()
            .AsNoTracking()
            .Where(epoch => epoch.EquipmentID == request.EquipmentId)
            .OrderByDescending(epoch => epoch.ID)
            .ToListAsync(cancellationToken);

        var currentMeterEpochId = epochs
            .FirstOrDefault(epoch => epoch.StatusID == 3)?.ID;

        var history = new List<MeterEpochRegisterRow>(epochs.Count);
        for (var i = 0; i < epochs.Count; i++)
        {
            var current = epochs[i];
            if (current.OpenningDate is not DateTime openingDate)
            {
                throw new InvalidOperationException(
                    $"Meter epoch {current.ID} for equipment {request.EquipmentId} has no opening date.");
            }

            var newerOpeningDate = i > 0 ? epochs[i - 1].OpenningDate : null;

            history.Add(new MeterEpochRegisterRow(
                EpochID: current.ID,
                OpenningDate: DateOnly.FromDateTime(openingDate),
                ClosingDate: newerOpeningDate is DateTime newerDate
                    ? DateOnly.FromDateTime(newerDate.AddDays(-1))
                    : null,
                ResetReadingAmount: current.ResetReadingAmount,
                OpeningReadingAmount: current.OpeningReadingAmount,
                ReasonID: current.ReasonID,
                ReasonName: null,
                EpochUsage: (current.ResetReadingAmount ?? 0) - (current.OpeningReadingAmount ?? 0),
                IsActive: current.IsActive,
                Remarks: current.Remarks));
        }

        return new EquipmentMeterRegisterDto(
            EquipmentID: request.EquipmentId,
            StoredCumulativeValue: cumulativeRecord.FinalReadingAmount,
            CurrentMeterEpochID: currentMeterEpochId,
            EpochHistory: history);
    }
}
