using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Commands;

public sealed record DeleteLabourCommand(long Id) : IRequest<bool>;
