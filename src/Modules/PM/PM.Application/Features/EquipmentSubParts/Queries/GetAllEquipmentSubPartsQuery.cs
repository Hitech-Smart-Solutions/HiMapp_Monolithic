using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Queries;

public sealed record GetAllEquipmentSubPartsQuery() : IRequest<IReadOnlyCollection<EquipmentSubPartDto>>;
