using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Models;
using Himapp.Workflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Workflow.Controllers;

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
    private readonly IWorkflowService _workflowService;
    private readonly ICurrentUser _currentUser;

    public WorkflowController(IWorkflowService workflowService, ICurrentUser currentUser)
    {
        _workflowService = workflowService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get the workflow status + history for an entity.
    /// </summary>
    [HttpGet("{entityName}/{entityId:int}")]
    public async Task<IActionResult> GetStatus(string entityName, int entityId, CancellationToken cancellationToken)
    {
        if (!DefaultWorkflowConfigurations.TryGetFor(entityName, out _)) return BadRequest(new { Message = "Unknown workflow entity." });
        var instance = await _workflowService.GetByEntityAsync(entityName, entityId, cancellationToken);
        if (instance is null)
            return NotFound(new { Message = $"No workflow found for {entityName}#{entityId}." });

        return Ok(new
        {
            instance.Id,
            instance.WorkflowType,
            instance.EntityName,
            instance.EntityId,
            instance.CurrentState,
            instance.CreatedAt,
            Config = _workflowService.GetConfiguration(entityName),
            History = instance.History.OrderBy(h => h.Level).Select(h => new
            {
                h.Level,
                h.Action,
                h.ActorUserId,
                h.Comment,
                h.ActionAt
            })
        });
    }

    /// <summary>
    /// Get all pending workflows.
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var pending = await _workflowService.GetPendingAsync(cancellationToken);
        return Ok(pending.Select(p => new
        {
            p.Id,
            p.EntityName,
            p.EntityId,
            p.CurrentState,
            p.CreatedAt
        }));
    }

    /// <summary>
    /// Approve an entity at the next level.
    /// </summary>
    [HttpPost("{entityName}/{entityId:int}/approve")]
    public async Task<IActionResult> Approve(string entityName, int entityId, [FromBody] WorkflowActionRequest request, CancellationToken cancellationToken)
    {
        return await ExecuteAction(entityName, entityId, WorkflowActions.Approve, request?.Comment, cancellationToken);
    }

    /// <summary>
    /// Reject an entity.
    /// </summary>
    [HttpPost("{entityName}/{entityId:int}/reject")]
    public async Task<IActionResult> Reject(string entityName, int entityId, [FromBody] WorkflowActionRequest request, CancellationToken cancellationToken)
    {
        return await ExecuteAction(entityName, entityId, WorkflowActions.Reject, request?.Comment, cancellationToken);
    }

    /// <summary>
    /// Cancel a workflow.
    /// </summary>
    [HttpPost("{entityName}/{entityId:int}/cancel")]
    public async Task<IActionResult> Cancel(string entityName, int entityId, [FromBody] WorkflowActionRequest request, CancellationToken cancellationToken)
    {
        return await ExecuteAction(entityName, entityId, WorkflowActions.Cancel, request?.Comment, cancellationToken);
    }

    /// <summary>
    /// Submit (or resubmit) an entity for approval.
    /// </summary>
    [HttpPost("{entityName}/{entityId:int}/submit")]
    public async Task<IActionResult> Submit(string entityName, int entityId, CancellationToken cancellationToken)
    {
        if (!DefaultWorkflowConfigurations.TryGetFor(entityName, out _)) return BadRequest(new { Message = "Unknown workflow entity." });
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var existing = await _workflowService.GetByEntityAsync(entityName, entityId, cancellationToken);
        if (existing is not null)
        {
            return BadRequest(new { Message = $"A workflow for {entityName}#{entityId} already exists (state: {existing.CurrentState})." });
        }

        var instance = await _workflowService.StartAsync(
            entityName, entityName, entityId, userId, cancellationToken);

        return Ok(new
        {
            instance.Id,
            instance.EntityName,
            instance.EntityId,
            instance.CurrentState,
            Message = $"Workflow started for {entityName}#{entityId}."
        });
    }

    private async Task<IActionResult> ExecuteAction(string entityName, int entityId, string action, string? comment, CancellationToken cancellationToken)
    {
        if (!DefaultWorkflowConfigurations.TryGetFor(entityName, out _)) return BadRequest(new { Message = "Unknown workflow entity." });
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var instance = await _workflowService.GetByEntityAsync(entityName, entityId, cancellationToken);
        if (instance is null)
            return NotFound(new { Message = $"No workflow found for {entityName}#{entityId}. Create one via POST .../submit first." });

        instance = await _workflowService.FireAsync(instance.Id, action, userId, comment, cancellationToken);

        return Ok(new
        {
            instance.Id,
            instance.CurrentState,
            Message = $"Action '{action}' executed successfully. New state: {instance.CurrentState}."
        });
    }
}

