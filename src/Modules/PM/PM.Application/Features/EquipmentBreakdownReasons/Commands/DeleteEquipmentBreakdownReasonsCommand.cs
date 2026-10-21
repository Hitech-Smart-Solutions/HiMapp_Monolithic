using MediatR;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Commands;

public sealed record DeleteEquipmentBreakdownReasonsCommand(int Id) : IRequest<bool>;
