using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;

public sealed record UpdateEquipmentChecklistPointCommand(
    int Id,
    string Description,
    bool IsActive,
    EquipmentChecklistPointType Type,
    int LastModifiedBy) : IRequest<EquipmentChecklistPointDto?>;
