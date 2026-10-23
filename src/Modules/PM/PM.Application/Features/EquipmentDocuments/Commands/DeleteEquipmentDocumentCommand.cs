using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Commands;

public sealed record DeleteEquipmentDocumentCommand(int Id) : IRequest<bool>;
