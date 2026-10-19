using MediatR;
using Himapp.PM.Application.Features.DocumentTypes.Models;

namespace Himapp.PM.Application.Features.DocumentTypes.Commands;

public sealed record UpdateDocumentTypeCommand(long Id, string DocumentTypeName, int LastModifiedBy, string? Remarks) : IRequest<DocumentTypeDto?>;
