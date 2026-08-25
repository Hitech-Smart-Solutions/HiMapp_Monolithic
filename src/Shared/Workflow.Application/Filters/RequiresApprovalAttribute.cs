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
    public int ProgramId { get; }
    public int Priority { get; }

    public RequiresApprovalAttribute(
        int programId,
        int priority = 1)
    {
        ProgramId = programId;
        Priority = priority;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executed = await next();

        if (executed.Result is not ObjectResult
            {
                StatusCode: >= 200 and <= 299
            } objectResult)
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

        var request = context.ActionArguments.Values
            .OfType<IWorkflowApprovalRequest>()
            .FirstOrDefault();

        if (request is null)
        {
            return;
        }

        var result = objectResult.Value as IWorkflowApprovalResult;

        if (result is null)
        {
            return;
        }

        var projectId = request.ProjectId;
        var statusId = request.StatusId;
        var remarks = request.Remarks;
        var id = result.Id;

        var nextApprover = await nextApproverService.GetNextApproverAsync(
            projectId,
            ProgramId,
            userId,
            Priority,
            context.HttpContext.RequestAborted);

        if (nextApprover is null)
        {
            if (statusId != 3)
            {
                statusId = 3;
            }
            else
            {
                return;
            }
        }

        await changeApprovalService.ChangeApprovalAsync(
            id,
            projectId,
            ProgramId,
            statusId,
            string.Empty,
            userId,
            nextApprover?.UserID ?? 0,
            nextApprover?.Priority   ?? 0,
            context.HttpContext.RequestAborted);
    }
}