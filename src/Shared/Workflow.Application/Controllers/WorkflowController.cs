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
    private readonly ICurrentUser _currentUser;

    public WorkflowController(
        IWorkflowGetNextApproverService workflowGetNextApproverService,
        IWorkflowChangeApprovalService workflowChangeApprovalService,
        ICurrentUser currentUser)
    {
        _workflowGetNextApproverService = workflowGetNextApproverService;
        _workflowChangeApprovalService = workflowChangeApprovalService;
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

        if (result is null)
        {
            return NotFound(new
            {
                Message = "No next approver found."
            });
        }

        return Ok(result);
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
}