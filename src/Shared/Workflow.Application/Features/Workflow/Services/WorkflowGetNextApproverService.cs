using Himapp.Workflow.Contracts.Models;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features.Workflow.Services;

public sealed class WorkflowGetNextApproverService
    : IWorkflowGetNextApproverService
{
    private readonly IWorkflowDbContext _context;

    public WorkflowGetNextApproverService(IWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<NextApproverModel?> GetNextApproverAsync(
        int projectId,
        int programId,
        int userId,
        int priority,
        CancellationToken cancellationToken)
    {
        return await _context
            .Set<NextApproverModel>()
            .FromSqlInterpolated($"""
                SELECT *
                FROM public.uspgetnextapproverforcentralizedcommonworkflow(
                    {projectId},
                    {programId},
                    {userId},
                    {priority}
                )
                """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
