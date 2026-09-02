using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries;

public sealed record GetAllDailyProgressQuery : IRequest<IReadOnlyCollection<DailyProgressModel>>;
public sealed record GetDailyProgressByIdQuery(int Id, int programId) : IRequest<DailyProgressByIDModel?>;
public sealed record GetDailyProgressListByProjectQuery(SearchParamsProjectWise SearchParams) : IRequest<DataSet>;
public sealed record GetActivityWiseQuantityByProjectQuery(int ProjectID, DateOnly ReportDate) : IRequest<List<ActivityWiseQuantityBySectionModel>>;
public sealed record GetDailyProgressByProjectAndDateQuery(int ProjectId, DateOnly ReportDate) : IRequest<DailyProgressModel?>;