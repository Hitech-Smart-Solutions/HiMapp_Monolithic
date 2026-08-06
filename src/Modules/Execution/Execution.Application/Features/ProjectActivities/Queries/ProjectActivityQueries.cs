using Himapp.Execution.Application.Features.ProjectActivities.Models;
using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetAllProjectActivitiesQuery(SearchParamsCompanyProjectWise SearchParams) : IRequest<DataSet>;
public sealed record GetProjectActivityByIdQuery(int Id) : IRequest<ProjectActivityModel?>;
public sealed record GetProjectActivityByProjectIdQuery(int ProjectId) : IRequest<List<ProjectActivityRefrenceModel>>;
