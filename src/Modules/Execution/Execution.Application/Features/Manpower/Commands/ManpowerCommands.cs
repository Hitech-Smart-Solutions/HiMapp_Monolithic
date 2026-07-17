using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;

namespace Himapp.Execution.Application.Features.Manpower.Commands;

public sealed record CreateManpowerCommand(CreateManpowerRequest Request) : IRequest<ManpowerModel>;
public sealed record UpdateManpowerCommand(long Id, UpdateManpowerRequest Request) : IRequest<ManpowerModel?>;
public sealed record DeleteManpowerCommand(long Id) : IRequest<bool>;
