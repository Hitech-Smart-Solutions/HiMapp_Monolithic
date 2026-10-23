using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Commands;

public sealed record DeleteSpecificationMasterCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
