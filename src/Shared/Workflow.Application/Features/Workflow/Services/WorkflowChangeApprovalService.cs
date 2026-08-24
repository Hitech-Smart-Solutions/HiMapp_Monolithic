using Himapp.Workflow.Contracts.Models;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features.Workflow.Services;

public sealed class WorkflowChangeApprovalService
    : IWorkflowChangeApprovalService
{
    private readonly IWorkflowDbContext _context;

    public WorkflowChangeApprovalService(IWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<ChangeApprovalModel?> ChangeApprovalAsync(
        int id,
        int projectId,
        int entityId,
        short statusId,
        string? remarks,
        int actionedBy,
        int nextApproverId,
        int priority,
        CancellationToken cancellationToken)
    {
        return await _context
            .Set<ChangeApprovalModel>()
            .FromSqlInterpolated($"""
                SELECT *
                FROM public."uspChangeApprovalForCentralizedCommonWorkFlow"(
                    {id},
                    {projectId},
                    {entityId},
                    {statusId},
                    {remarks},
                    {actionedBy},
                    {nextApproverId},
                    {priority}
                )
                """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
