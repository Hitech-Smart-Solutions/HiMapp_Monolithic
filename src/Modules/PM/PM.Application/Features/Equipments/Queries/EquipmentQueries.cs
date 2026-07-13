using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Queries;

public sealed record GetAllEquipmentsQuery : IRequest<IReadOnlyCollection<EquipmentDto>>;
public sealed record GetEquipmentByIdQuery(long Id) : IRequest<EquipmentDto?>;
