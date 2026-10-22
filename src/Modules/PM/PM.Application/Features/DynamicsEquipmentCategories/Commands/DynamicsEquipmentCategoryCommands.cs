using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories.Commands;

public sealed record CreateDynamicsEquipmentCategoryCommand(DynamicsEquipmentCategoryRequest Request) : IRequest<DynamicsEquipmentCategoryDto>;

public sealed record UpdateDynamicsEquipmentCategoryCommand(int Id, DynamicsEquipmentCategoryRequest Request) : IRequest<DynamicsEquipmentCategoryDto?>;

/// <summary>Delete (soft delete to be enabled once IsActive is migrated).</summary>
public sealed record DeleteDynamicsEquipmentCategoryCommand(int Id) : IRequest<bool>;
