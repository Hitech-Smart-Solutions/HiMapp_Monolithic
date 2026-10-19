using MediatR;
using Himapp.PM.Application.Features.DocumentTypes.Models;

namespace Himapp.PM.Application.Features.DocumentTypes.Commands;

public sealed record CreateDocumentTypeCommand(string DocumentTypeName, int CreateBy, int LastModifiedBy, string? Remarks) : IRequest<DocumentTypeDto>;
