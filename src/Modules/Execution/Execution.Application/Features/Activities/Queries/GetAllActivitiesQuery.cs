using MediatR;
using Himapp.Execution.Application.Features;
using System.Data;

namespace Himapp.Execution.Application.Features.Activities.Queries;

public sealed record GetAllActivitiesQuery(SearchParams SearchParams) : IRequest<DataSet>;
