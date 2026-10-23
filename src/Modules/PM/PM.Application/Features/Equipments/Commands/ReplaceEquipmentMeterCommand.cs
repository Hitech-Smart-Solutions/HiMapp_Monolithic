using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Commands;

public sealed record ReplaceEquipmentMeterCommand(int EquipmentId, MeterReplacementRequestDto Request)
    : IRequest<MeterReplacementResponseDto?>;
