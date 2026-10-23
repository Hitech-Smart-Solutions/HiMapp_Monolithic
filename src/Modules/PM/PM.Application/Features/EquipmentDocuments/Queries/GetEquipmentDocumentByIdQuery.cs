using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Queries;

public sealed record GetEquipmentDocumentByIdQuery(int Id) : IRequest<EquipmentDocumentDto?>;
