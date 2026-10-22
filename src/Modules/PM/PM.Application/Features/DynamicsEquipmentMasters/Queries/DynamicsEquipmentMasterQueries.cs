using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Queries;

public sealed record GetDynamicsEquipmentMastersQuery(int Page, int PageSize, string? Search)
    : IRequest<PagedResult<DynamicsEquipmentMasterDto>>;

public sealed record GetDynamicsEquipmentMasterByIdQuery(int Id) : IRequest<DynamicsEquipmentMasterDto?>;
