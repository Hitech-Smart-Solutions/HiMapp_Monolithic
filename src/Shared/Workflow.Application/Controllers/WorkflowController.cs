using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Application.Features.Workflow.Models;
using Himapp.Workflow.Application.Features.Workflow.Services;
using Himapp.Workflow.Contracts.References;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Controllers;

/// <summary>
/// Shared / generic workflow approval controller.
/// Any entity type with a workflow can be approved/rejected via these endpoints.
/// 
/// POST /api/workflow/{entityName}/{entityId}/approve
/// POST /api/workflow/{entityName}/{entityId}/reject
/// GET  /api/workflow/{entityName}/{entityId}          — get workflow status
/// GET  /api/workflow/pending                           — get all pending workflows
/// </summary>
[ApiController]
[Authorize]
[Route("api/workflow")]
public sealed class WorkflowController : ControllerBase
{
    private readonly IWorkflowGetNextApproverService _workflowGetNextApproverService;
    private readonly IWorkflowChangeApprovalService _workflowChangeApprovalService;
    private readonly IWorkflowPendingApprovalsService _workflowPendingApprovalsService;
    private readonly IWorkflowDisApproveTransactionService _workflowDisApproveTransactionService;
    private readonly ICurrentUser _currentUser;

    public WorkflowController(
        IWorkflowGetNextApproverService workflowGetNextApproverService,
        IWorkflowChangeApprovalService workflowChangeApprovalService,
        IWorkflowPendingApprovalsService workflowPendingApprovalsService,
        IWorkflowDisApproveTransactionService workflowDisApproveTransactionService,
        ICurrentUser currentUser)
    {
        _workflowGetNextApproverService = workflowGetNextApproverService;
        _workflowChangeApprovalService = workflowChangeApprovalService;
        _workflowPendingApprovalsService = workflowPendingApprovalsService;
        _workflowDisApproveTransactionService = workflowDisApproveTransactionService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Gets the next approver for a centralized common workflow.
    /// </summary>
    [HttpGet("next-approver")]
    public async Task<IActionResult> GetNextApprover(
        [FromQuery] int projectId,
        [FromQuery] int programId,
        [FromQuery] int priority,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? 0;

        var result = await _workflowGetNextApproverService.GetNextApproverAsync(
            projectId,
            programId,
            userId,
            priority,
            cancellationToken);
    
        return Ok(result);
    }

    /// <summary>
    /// Disapproves a transaction in the centralized common workflow.
    /// </summary>
    [HttpPost("disapprove-transaction")]
    public async Task<IActionResult> DisApproveTransaction(
        [FromQuery] int id,
        [FromQuery] int programId,
        [FromQuery] string disApprovalRemarks,
        [FromQuery] int remarksId = 0,
        CancellationToken cancellationToken = default)
    {
        var actionedBy = _currentUser.UserId ?? 0;

        await _workflowDisApproveTransactionService.DisApproveTransactionAsync(
            id,
            programId,
            disApprovalRemarks,
            actionedBy,
            remarksId,
            cancellationToken);

        return Ok(new
        {
            Message = "Transaction disapproved successfully."
        });
    }

    /// <summary>
    /// Changes approval status for a centralized common workflow.
    /// </summary>
    [HttpPost("change-approval")]
    public async Task<IActionResult> ChangeApproval(
        [FromBody] ChangeApprovalRequest request,
        CancellationToken cancellationToken)
    {
        var actionedBy = _currentUser.UserId ?? 0;

        var result = await _workflowChangeApprovalService.ChangeApprovalAsync(
            request.Id,
            request.ProjectId,
            request.EntityId,
            request.StatusId,
            request.Remarks,
            actionedBy,
            request.NextApproverId,
            request.Priority,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("pending/daily-progress")]
    public async Task<IActionResult> GetAwaitingDailyProgress(
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? 0;

        if (userId <= 0)
        {
            return Unauthorized(new
            {
                Message = "Invalid or missing user."
            });
        }

        var result =
            await _workflowPendingApprovalsService.GetAwaitingDailyProgress(
                userId,
                cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets all departmental labour slips awaiting approval
    /// for the currently logged-in user.
    /// </summary>
    [HttpGet("pending/departmental-labour-slip")]
    public async Task<IActionResult> GetAwaitingDepartmentalLabourSlip(
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? 0;

        if (userId <= 0)
        {
            return Unauthorized(new
            {
                Message = "Invalid or missing user."
            });
        }

        var result =
            await _workflowPendingApprovalsService
                .GetAwaitingDepartmentalLabourSlip(
                    userId,
                    cancellationToken);

        return Ok(result);
    }
}