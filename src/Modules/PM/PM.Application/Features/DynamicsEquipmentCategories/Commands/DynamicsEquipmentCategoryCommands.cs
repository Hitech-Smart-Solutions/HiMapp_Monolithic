using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories.Commands;

public sealed record CreateDynamicsEquipmentCategoryCommand(DynamicsEquipmentCategoryRequest Request) : IRequest<DynamicsEquipmentCategoryDto>;

public sealed record UpdateDynamicsEquipmentCategoryCommand(int Id, DynamicsEquipmentCategoryRequest Request) : IRequest<DynamicsEquipmentCategoryDto?>;

/// <summary>Soft delete: sets IsActive = false, the row is kept.</summary>
public sealed record DeleteDynamicsEquipmentCategoryCommand(int Id) : IRequest<bool>;
