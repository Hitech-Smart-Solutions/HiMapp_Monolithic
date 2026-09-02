using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;
using MediatR;
using System.Data;

namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Queries;

public sealed record GetAllCentralUserRoleMappingsQuery : IRequest<IReadOnlyCollection<CentralUserRoleMappingDto>>;
public sealed record GetCentralUserRoleMappingByIdQuery(int Id) : IRequest<CentralUserRoleMappingDto?>;
public sealed record GetRoleMappingListByCompanyQuery(SearchParams SearchParams) : IRequest<DataSet>;