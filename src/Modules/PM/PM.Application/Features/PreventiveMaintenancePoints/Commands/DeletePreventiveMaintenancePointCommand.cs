using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;

public sealed record DeletePreventiveMaintenancePointCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
