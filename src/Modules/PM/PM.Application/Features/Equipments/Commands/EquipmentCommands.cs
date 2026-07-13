using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Commands;

public sealed record CreateEquipmentCommand(long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn) : IRequest<EquipmentDto>;
public sealed record UpdateEquipmentCommand(long Id, long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn) : IRequest<EquipmentDto?>;
public sealed record DeleteEquipmentCommand(long Id) : IRequest<bool>;
