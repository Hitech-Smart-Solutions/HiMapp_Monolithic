using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Commands;

public sealed record CreateEquipmentBreakdownReasonsCommand(
    string BreakdownReason,
    bool IsActive,
    EquipmentBreakdownReasonType Type,
    int CreateBy,
    int LastModifiedBy) : IRequest<EquipmentBreakdownReasonsDto>;
