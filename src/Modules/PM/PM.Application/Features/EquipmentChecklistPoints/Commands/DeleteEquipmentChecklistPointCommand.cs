using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;

public sealed record DeleteEquipmentChecklistPointCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
