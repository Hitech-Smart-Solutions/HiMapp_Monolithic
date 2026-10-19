using MediatR;
using Himapp.PM.Application.Features.DocumentTypes.Models;

namespace Himapp.PM.Application.Features.DocumentTypes.Queries;

public sealed record GetAllDocumentTypesQuery() : IRequest<IReadOnlyCollection<DocumentTypeDto>>;
