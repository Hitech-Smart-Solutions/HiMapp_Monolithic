using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Commands;

public sealed record CreateEquipmentDocumentCommand(
    int DocumentTypeID,
    string DocumentName,
    DateTime StartDate,
    DateTime EndDate,
    bool? IsActive,
    int CreateBy,
    int LastModifiedBy) : IRequest<EquipmentDocumentDto>;
