using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Commands;

public sealed record CreateDynamicsEquipmentMasterCommand(DynamicsEquipmentMasterRequest Request) : IRequest<DynamicsEquipmentMasterDto>;

public sealed record UpdateDynamicsEquipmentMasterCommand(int Id, DynamicsEquipmentMasterRequest Request) : IRequest<DynamicsEquipmentMasterDto?>;

/// <summary>Delete (soft delete to be enabled once IsActive is migrated).</summary>
public sealed record DeleteDynamicsEquipmentMasterCommand(int Id) : IRequest<bool>;
