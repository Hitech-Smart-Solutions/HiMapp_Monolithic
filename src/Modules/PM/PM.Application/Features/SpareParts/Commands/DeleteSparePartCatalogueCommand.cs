using MediatR;

namespace Himapp.PM.Application.Features.SpareParts.Commands;

public sealed record DeleteSparePartCatalogueCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
