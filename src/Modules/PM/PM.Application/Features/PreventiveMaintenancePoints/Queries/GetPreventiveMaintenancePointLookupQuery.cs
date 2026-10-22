using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Queries;

public sealed record GetPreventiveMaintenancePointLookupQuery() : IRequest<IReadOnlyList<PreventiveMaintenancePointLookupResponse>>;