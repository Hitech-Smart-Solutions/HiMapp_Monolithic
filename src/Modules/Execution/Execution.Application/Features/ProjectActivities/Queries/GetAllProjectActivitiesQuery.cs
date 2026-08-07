using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetAllProjectActivitiesQuery(SearchParamsCompanyProjectWise SearchParams) : IRequest<DataSet>;
