using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Queries;

public sealed record GetAllEquipmentDocumentsQuery() : IRequest<IReadOnlyCollection<EquipmentDocumentDto>>;
