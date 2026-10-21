using MediatR;

namespace Himapp.PM.Application.Features.WorkDescriptions.Commands;

public sealed record DeleteWorkDescriptionCommand(long Id) : IRequest<bool>;
