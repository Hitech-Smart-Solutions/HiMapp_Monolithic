using Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;
using MediatR;
using System.Data;

namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Queries;

public sealed record GetAllApprovalWorkflowsQuery : IRequest<IReadOnlyCollection<ApprovalWorkflowDto>>;
public sealed record GetApprovalWorkflowByIdQuery(int Id) : IRequest<ApprovalWorkflowDto?>;
public sealed record GetWorkflowListByProjectQuery(SearchParamsProjectWise SearchParams) : IRequest<DataSet>;
