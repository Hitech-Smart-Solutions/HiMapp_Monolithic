using MediatR;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Commands;

public sealed record DeleteEquipmentSubPartCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
