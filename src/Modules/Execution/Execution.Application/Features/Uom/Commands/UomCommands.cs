using MediatR;
using Himapp.Execution.Application.Features.Uom.Models;

namespace Himapp.Execution.Application.Features.Uom.Commands;

public sealed record CreateUomCommand(CreateUomRequest Request) : IRequest<UomModel>;
public sealed record UpdateUomCommand(long Id, UpdateUomRequest Request) : IRequest<UomModel?>;
public sealed record DeleteUomCommand(long Id) : IRequest<bool>;
