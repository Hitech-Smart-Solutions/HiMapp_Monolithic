using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Commands;

public sealed record DeleteEquipmentDocumentCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
