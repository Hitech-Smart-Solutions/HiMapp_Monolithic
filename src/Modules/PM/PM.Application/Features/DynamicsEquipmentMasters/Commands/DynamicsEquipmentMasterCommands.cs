using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Commands;

public sealed record CreateDynamicsEquipmentMasterCommand(DynamicsEquipmentMasterRequest Request) : IRequest<DynamicsEquipmentMasterDto>;

public sealed record UpdateDynamicsEquipmentMasterCommand(int Id, DynamicsEquipmentMasterRequest Request) : IRequest<DynamicsEquipmentMasterDto?>;

/// <summary>Soft delete: sets IsActive = false, the row is kept.</summary>
public sealed record DeleteDynamicsEquipmentMasterCommand(int Id) : IRequest<bool>;
