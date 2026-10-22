using Himapp.PM.Application.Features.Common;
using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Queries;

public sealed record GetAllEquipmentsQuery : IRequest<IReadOnlyCollection<EquipmentDto>>;
public sealed record GetEquipmentByIdQuery(long Id) : IRequest<EquipmentDto?>;

public sealed record GetEquipmentPMConfigurationsByEquipmentQuery(int EquipmentId)
    : IRequest<IReadOnlyList<EquipmentPMConfigurationResponse>>;

public sealed record GetEquipmentPMConfigurationByIdQuery(int EquipmentId, long Id)
    : IRequest<EquipmentPMConfigurationResponse?>;

public sealed record GetAwaitingPMConfigurationQueueQuery(
    int Page,
    int PageSize,
    int? CategoryId,
    string? Search)
    : IRequest<PagedResult<AwaitingPMConfigurationRow>>;


public sealed record GetMeterEpochRegisterQuery(int EquipmentId) : IRequest<IReadOnlyList<MeterEpochRegisterRow>>;