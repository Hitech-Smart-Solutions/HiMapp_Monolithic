using Himapp.PM.Application.Features.Equipments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.Equipments.Queries;

public sealed record GetEquipmentMeterRegisterSummaryQuery(int EquipmentId)
    : IRequest<EquipmentMeterRegisterDto?>;
