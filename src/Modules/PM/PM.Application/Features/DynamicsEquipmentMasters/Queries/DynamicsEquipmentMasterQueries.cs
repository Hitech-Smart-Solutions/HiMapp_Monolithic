using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Queries;

public sealed record GetDynamicsEquipmentMastersQuery(ListSearchParams Params)
    : IRequest<PagedResult<DynamicsEquipmentMasterDto>>;

public sealed record GetDynamicsEquipmentMasterByIdQuery(int Id) : IRequest<DynamicsEquipmentMasterDto?>;
