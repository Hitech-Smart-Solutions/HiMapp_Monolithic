using MediatR;
using Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Commands;

public sealed record CreateApprovalWorkflowCommand(CreateApprovalWorkflowRequest Request) : IRequest<ApprovalWorkflowDto>;
public sealed record UpdateApprovalWorkflowCommand(int Id, UpdateApprovalWorkflowRequest Request) : IRequest<ApprovalWorkflowDto?>;
public sealed record DeleteApprovalWorkflowCommand(int Id, AddTransactionActionHistoryDTO actionHistory) : IRequest<bool>;
