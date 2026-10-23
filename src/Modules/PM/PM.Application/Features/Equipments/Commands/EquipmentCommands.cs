using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Commands;

public sealed record CreateEquipmentCommand(long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn) : IRequest<EquipmentDto>;
public sealed record UpdateEquipmentCommand(long Id, long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn) : IRequest<EquipmentDto?>;
public sealed record DeleteEquipmentCommand(long Id) : IRequest<bool>;
public sealed record CreateEquipmentPMConfigurationCommand(
    int EquipmentId,
    EquipmentPMConfigurationRequest Request)
    : IRequest<EquipmentPMConfigurationResponse>;

public sealed record UpdateEquipmentPMConfigurationCommand(
    int EquipmentId,
    long Id,
    EquipmentPMConfigurationRequest Request)
    : IRequest<EquipmentPMConfigurationResponse?>;