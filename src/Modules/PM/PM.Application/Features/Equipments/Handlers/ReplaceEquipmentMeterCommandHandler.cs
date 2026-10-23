using System.Data;
using Himapp.PM.Application.Features.Equipments.Commands;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.Equipments.Handlers;

public sealed class ReplaceEquipmentMeterCommandHandler
    : IRequestHandler<ReplaceEquipmentMeterCommand, MeterReplacementResponseDto?>
{
    private const int CurrentEpochStatusId = 3;
    private const int ClosedEpochStatusId = 4;

    private readonly IPMDbContext _db;

    public ReplaceEquipmentMeterCommandHandler(IPMDbContext db) => _db = db;

    public async Task<MeterReplacementResponseDto?> Handle(
        ReplaceEquipmentMeterCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        ValidateRequest(command.EquipmentId, request);
        var replacementUtc = request.ReplacementDate.ToUniversalTime();

        var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        await using (transaction)
        {
            var equipmentExists = await _db.Set<DynamicsEquipmentMaster>()
                .AnyAsync(equipment => equipment.ID == command.EquipmentId, cancellationToken);
            if (!equipmentExists)
                return null;

            var activeEpochs = await _db.Set<EquipmentMeterEpochDetails>()
                .Where(epoch => epoch.EquipmentID == command.EquipmentId && epoch.IsActive)
                .ToListAsync(cancellationToken);

            if (activeEpochs.Count != 1)
                throw new InvalidOperationException(
                    activeEpochs.Count == 0
                        ? $"Equipment {command.EquipmentId} has no active meter epoch."
                        : $"Equipment {command.EquipmentId} has more than one active meter epoch.");

            var oldEpoch = activeEpochs[0];
            if (oldEpoch.StatusID != CurrentEpochStatusId)
                throw new InvalidOperationException(
                    $"Active meter epoch {oldEpoch.ID} does not have current status {CurrentEpochStatusId}.");

            if (oldEpoch.ID != request.CurrentMeterEpochID)
                throw new InvalidOperationException(
                    $"Meter epoch {request.CurrentMeterEpochID} is no longer current for equipment {command.EquipmentId}.");

            if (!oldEpoch.IsActive)
                throw new InvalidOperationException($"Current meter epoch {oldEpoch.ID} is inactive.");

            if (!oldEpoch.OpeningReadingAmount.HasValue)
                throw new InvalidOperationException(
                    $"Current meter epoch {oldEpoch.ID} has no opening reading.");

            if (!oldEpoch.OpenningDate.HasValue
                || replacementUtc < oldEpoch.OpenningDate.Value)
                throw new InvalidOperationException(
                    "ReplacementDate cannot be earlier than the current meter epoch opening date.");

            if (request.OldMeterFinalReading!.Value < oldEpoch.OpeningReadingAmount.Value)
                throw new InvalidOperationException(
                    "The old meter final reading cannot be less than its opening reading.");

            var now = DateTime.UtcNow;

            oldEpoch.ResetReadingAmount = request.OldMeterFinalReading.Value;
            oldEpoch.StatusID = ClosedEpochStatusId;
            oldEpoch.IsActive = false;
            oldEpoch.LastModifiedDate = now;

            var newEpoch = new EquipmentMeterEpochDetails
            {
                EquipmentID = command.EquipmentId,
                OpenningDate = replacementUtc,
                MeterReadingID = request.MeterReadingID,
                OpeningReadingAmount = request.OpeningReadingAmount!.Value,
                ReasonID = request.ReasonID,
                Remarks = request.Remarks,
                StatusID = CurrentEpochStatusId,
                IsActive = true,
                CreatedDate = now
            };

            _db.Set<EquipmentMeterEpochDetails>().Add(newEpoch);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new MeterReplacementResponseDto
            {
                EquipmentID = command.EquipmentId,
                ClosedMeterEpochID = oldEpoch.ID,
                NewMeterEpochID = newEpoch.ID,
                ClosedMeterStatusID = ClosedEpochStatusId,
                CurrentMeterStatusID = CurrentEpochStatusId,
                NewMeterOpeningReadingAmount = request.OpeningReadingAmount.Value,
                CumulativeEquipmentHours = null,
                Message = "Meter replacement recorded."
            };
        }
    }

    private static void ValidateRequest(int equipmentId, MeterReplacementRequestDto request)
    {
        if (equipmentId <= 0)
            throw new InvalidOperationException("A valid equipment ID is required.");

        if (request.EquipmentID != equipmentId)
            throw new InvalidOperationException("The equipment ID in the route and request must match.");

        if (request.CurrentMeterEpochID <= 0)
            throw new InvalidOperationException("A valid current meter epoch ID is required.");

        if (request.ReplacementDate == default || request.ReplacementDate.Kind == DateTimeKind.Unspecified)
            throw new InvalidOperationException("ReplacementDate must include a UTC marker or an explicit time-zone offset.");

        if (!request.OldMeterFinalReading.HasValue || request.OldMeterFinalReading.Value < 0)
            throw new InvalidOperationException("A non-negative old meter final reading is required.");

        if (!request.OpeningReadingAmount.HasValue || request.OpeningReadingAmount.Value < 0)
            throw new InvalidOperationException("A non-negative new meter opening reading is required.");

        if (request.MeterReadingID is <= 0)
            throw new InvalidOperationException("MeterReadingID must be positive when supplied.");

        if (request.ReasonID is <= 0)
            throw new InvalidOperationException("ReasonID must be positive when supplied.");
    }
}
