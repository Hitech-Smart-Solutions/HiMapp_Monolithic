using MediatR;

namespace Himapp.PM.Application.Features.DocumentTypes.Commands;

public sealed record DeleteDocumentTypeCommand(long Id) : IRequest<bool>;
