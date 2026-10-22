using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Commands;

public sealed record UpdateEquipmentBreakdownReasonsCommand(
    int Id,
    string BreakdownReason,
    bool IsActive,
    EquipmentBreakdownReasonType Type,
    int LastModifiedBy) : IRequest<EquipmentBreakdownReasonsDto?>;
