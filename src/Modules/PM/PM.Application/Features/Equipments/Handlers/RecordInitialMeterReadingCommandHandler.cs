using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Himapp.PM.Application.Features.Equipments.Commands;
using Himapp.PM.Application.Features.Equipments.Models;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.Equipments.Handlers;

public sealed class RecordInitialMeterReadingCommandHandler
    : IRequestHandler<RecordInitialMeterReadingCommand, int?>
{
    private const int CurrentEpochStatusId = 3;
    private static readonly Regex ExplicitTimezoneSuffix = new(@"(?:Z|[+-]\d{2}:\d{2})$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly IPMDbContext _db;

    public RecordInitialMeterReadingCommandHandler(IPMDbContext db) => _db = db;

    public async Task<int?> Handle(
        RecordInitialMeterReadingCommand command,
        CancellationToken cancellationToken)
    {
        var openingDateUtc = ValidateRequest(command.EquipmentId, command.Request);

        await using var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var equipmentExists = await _db.Set<DynamicsEquipmentMaster>()
            .AnyAsync(equipment => equipment.ID == command.EquipmentId, cancellationToken);
        if (!equipmentExists)
            return null;

        var readingTypeIsActive = await _db.Set<MeterReadingType>()
            .AsNoTracking()
            .Where(readingType => readingType.ID == command.Request.MeterReadingID!.Value)
            .Select(readingType => (bool?)readingType.IsActive)
            .SingleOrDefaultAsync(cancellationToken);

        if (readingTypeIsActive is null)
            throw new ArgumentException($"Meter reading type {command.Request.MeterReadingID} was not found.");

        if (!readingTypeIsActive.Value)
            throw new ArgumentException($"Meter reading type {command.Request.MeterReadingID} is inactive.");

        var hasActiveEpoch = await _db.Set<EquipmentMeterEpochDetails>()
            .AnyAsync(epoch => epoch.EquipmentID == command.EquipmentId
                            && (epoch.IsActive || epoch.StatusID == CurrentEpochStatusId),
                cancellationToken);

        if (hasActiveEpoch)
            throw new InvalidOperationException(
                $"Equipment {command.EquipmentId} already has an active meter epoch.");

        var now = DateTime.UtcNow;
        var epoch = new EquipmentMeterEpochDetails
        {
            EquipmentID = command.EquipmentId,
            OpenningDate = openingDateUtc,
            OpeningReadingAmount = command.Request.OpeningReadingAmount!.Value,
            MeterReadingID = command.Request.MeterReadingID!.Value,
            StatusID = CurrentEpochStatusId,
            IsActive = true,
            CreatedDate = now,
            LastModifiedDate = now
        };

        _db.Set<EquipmentMeterEpochDetails>().Add(epoch);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return epoch.ID;
    }

    private static DateTime ValidateRequest(int equipmentId, InitialMeterReadingRequestDto request)
    {
        if (equipmentId <= 0)
            throw new ArgumentException("A valid equipment ID is required.");

        if (request.OpeningReadingAmount is not decimal openingReadingAmount
            || openingReadingAmount < 0)
            throw new ArgumentException("A non-negative opening reading amount is required.");

        if (request.MeterReadingID is not int meterReadingId || meterReadingId <= 0)
            throw new ArgumentException("A valid meter reading type ID is required.");

        if (string.IsNullOrWhiteSpace(request.OpeningDate)
            || !ExplicitTimezoneSuffix.IsMatch(request.OpeningDate)
            || !DateTimeOffset.TryParse(
                request.OpeningDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var openingDate))
        {
            throw new ArgumentException(
                "OpeningDate must be a valid timestamp with a UTC marker or explicit time-zone offset.");
        }

        return openingDate.UtcDateTime;
    }
}
