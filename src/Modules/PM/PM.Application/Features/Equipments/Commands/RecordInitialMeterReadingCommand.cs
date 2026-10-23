using Himapp.PM.Application.Features.Equipments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Commands;

public sealed record RecordInitialMeterReadingCommand(
    int EquipmentId,
    InitialMeterReadingRequestDto Request) : IRequest<int?>;
