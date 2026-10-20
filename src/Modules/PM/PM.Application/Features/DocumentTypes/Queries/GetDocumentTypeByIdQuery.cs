using MediatR;
using Himapp.PM.Application.Features.DocumentTypes.Models;

namespace Himapp.PM.Application.Features.DocumentTypes.Queries;

public sealed record GetDocumentTypeByIdQuery(long Id) : IRequest<DocumentTypeDto?>;
