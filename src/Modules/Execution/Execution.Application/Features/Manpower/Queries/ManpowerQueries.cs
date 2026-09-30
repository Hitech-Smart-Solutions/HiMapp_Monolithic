using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;
using System.Data;
using DocumentFormat.OpenXml.Bibliography;

namespace Himapp.Execution.Application.Features.Manpower.Queries;

public sealed record GetAllManpowersQuery : IRequest<IReadOnlyCollection<ManpowerModel>>;
public sealed record GetManpowerByIdQuery(long Id) : IRequest<ManpowerModel?>;
public sealed record GetManpowerByProjectID(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<DataSet>;
public sealed record GetLastManpowerBySectionIDQuery(int ProjectId, int SectionId) : IRequest<ManpowerModel?>;
public sealed record GetManpowerBySectionProjectAndDateQuery(int ProjectId, int SectionId, DateOnly entryDate) : IRequest<ManpowerModel?>;
public sealed record GetManpowerDashboard(ManpowerDashboardSearchParams SearchParams) : IRequest<DataSet>;
public sealed record GetActivityWisePlanVsAchievement(ManpowerDashboardSearchParams SearchParams) : IRequest<DataSet>;
public sealed record GetActivityWiseProductivity(ManpowerDashboardSearchParams SearchParams) : IRequest<DataSet>;
public sealed record GetLabourCostBudgetVsActual(ManpowerDashboardSearchParams SearchParams) : IRequest<DataSet>;

