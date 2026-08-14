using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;

namespace Himapp.Execution.Application.Features.Manpower.Commands;

public sealed record CreateManpowerCommand(CreateManpowerRequest Request) : IRequest<ManpowerModel>;
public sealed record UpdateManpowerCommand(int Id, UpdateManpowerRequest Request) : IRequest<ManpowerModel?>;
public sealed record DeleteManpowerCommand(int Id) : IRequest<bool>;
public sealed record DeleteManpowerActionCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
