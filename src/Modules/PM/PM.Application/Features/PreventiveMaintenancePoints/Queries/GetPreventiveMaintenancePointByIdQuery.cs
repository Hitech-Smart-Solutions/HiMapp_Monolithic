using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Queries;

public sealed record GetPreventiveMaintenancePointByIdQuery(long Id) : IRequest<PreventiveMaintenancePointDto?>;
