using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;

public sealed record CreateEquipmentChecklistPointCommand(
    string Description,
    bool IsActive,
    EquipmentChecklistPointType Type,
    int CreateBy,
    int LastModifiedBy) : IRequest<EquipmentChecklistPointDto>;
