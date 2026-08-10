using MediatR;
using Himapp.Execution.Application.Features.Planning.Models;
using System.Data;

namespace Himapp.Execution.Application.Features.Planning.Queries;

public sealed record GetAllPlanningsQuery : IRequest<IReadOnlyCollection<PlanningModel>>;
public sealed record GetPlanningByIdQuery(long Id) : IRequest<PlanningModel?>;
public sealed record GetPlanningListByProjectQuery(SearchParamsProjectWise SearchParams) : IRequest<DataSet>;
public sealed record DownloadPlanningTemplateQuery(int ProjectId) : IRequest<byte[]>;