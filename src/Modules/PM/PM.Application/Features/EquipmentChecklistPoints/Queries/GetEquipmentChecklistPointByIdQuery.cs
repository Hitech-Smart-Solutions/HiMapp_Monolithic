using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Queries;

public sealed record GetEquipmentChecklistPointByIdQuery(int Id) : IRequest<EquipmentChecklistPointDto?>;
