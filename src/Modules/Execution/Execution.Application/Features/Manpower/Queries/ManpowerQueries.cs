using MediatR;
using Himapp.Execution.Application.Features.Manpower.Models;
using System.Data;

namespace Himapp.Execution.Application.Features.Manpower.Queries;

public sealed record GetAllManpowersQuery : IRequest<IReadOnlyCollection<ManpowerModel>>;
public sealed record GetManpowerByIdQuery(long Id) : IRequest<ManpowerModel?>;
public sealed record GetManpowerByProjectID(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<DataSet>;
