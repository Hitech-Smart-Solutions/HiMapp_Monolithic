using Himapp.Execution.Application.Features.DailyProgress.Models;
using MediatR;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries;

public sealed record GetAllDailyProgressQuery : IRequest<IReadOnlyCollection<DailyProgressModel>>;
public sealed record GetDailyProgressByIdQuery(int Id) : IRequest<DailyProgressModel?>;
public sealed record GetDailyProgressListByProjectQuery(SearchParamsProjectWise SearchParams) : IRequest<DataSet>;
