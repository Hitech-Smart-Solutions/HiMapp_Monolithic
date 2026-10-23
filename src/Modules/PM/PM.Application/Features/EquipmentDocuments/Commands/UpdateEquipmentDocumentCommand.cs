using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Commands;

public sealed record UpdateEquipmentDocumentCommand(
    int Id,
    int DocumentTypeID,
    string DocumentName,
    DateTime StartDate,
    DateTime EndDate,
    bool? IsActive,
    int LastModifiedBy) : IRequest<EquipmentDocumentDto>;
