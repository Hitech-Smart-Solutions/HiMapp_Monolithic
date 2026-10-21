using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;

public sealed record DeleteEquipmentChecklistPointCommand(int Id) : IRequest<bool>;
