using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;

namespace Himapp.Execution.Application.Features.Manpower.Queries;

public sealed record GetAllManpowersQuery : IRequest<IReadOnlyCollection<ManpowerModel>>;
public sealed record GetManpowerByIdQuery(long Id) : IRequest<ManpowerModel?>;
public sealed record GetManpowerByProjectID(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<PagedResult<ManpowerModel>>;
public sealed record GetLastManpowerBySectionIDQuery(int ProjectId, int SectionId) : IRequest<ManpowerModel?>;
