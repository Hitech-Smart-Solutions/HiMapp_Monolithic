using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Application.Features.Workflow.Models;
using Himapp.Workflow.Application.Features.Workflow.Services;
using Himapp.Workflow.Contracts.References;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow.Application.Filters;

/// <summary>
/// Automatically processes workflow approval after a successful request.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true)]
public sealed class RequiresApprovalAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executed = await next();

        // Process workflow only when the API request was successful.
        if (executed.Result is not ObjectResult
            {
                StatusCode: >= 200 and <= 299
            } objectResult)
        {
            return;
        }

        if (objectResult.Value is not IRequiresApproval approvalDto)
        {
            return;
        }

        var nextApproverService =
            context.HttpContext.RequestServices
                .GetRequiredService<IWorkflowGetNextApproverService>();

        var changeApprovalService =
            context.HttpContext.RequestServices
                .GetRequiredService<IWorkflowChangeApprovalService>();

        var currentUser =
            context.HttpContext.RequestServices
                .GetRequiredService<ICurrentUser>();

        var userId = currentUser.UserId ?? 0;

        // Get the next approver.
        var nextApprover = await nextApproverService.GetNextApproverAsync(
            approvalDto.ProjectId,
            approvalDto.ProgramId,
            userId,
            approvalDto.Priority,
            context.HttpContext.RequestAborted);

        if (nextApprover is null)
        {
            return;
        }

        // Change workflow approval status.
        await changeApprovalService.ChangeApprovalAsync(
            approvalDto.Id,
            approvalDto.ProjectId,
            approvalDto.EntityId,
            approvalDto.StatusId,
            approvalDto.Remarks,
            userId,
            nextApprover.ApproverId,
            approvalDto.Priority,
            context.HttpContext.RequestAborted);
    }
}