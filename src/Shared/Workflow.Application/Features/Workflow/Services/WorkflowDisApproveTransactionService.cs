using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features.Workflow.Services;

public sealed class WorkflowDisApproveTransactionService
    : IWorkflowDisApproveTransactionService
{
    private readonly IWorkflowDbContext _context;

    public WorkflowDisApproveTransactionService(
        IWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task DisApproveTransactionAsync(
        int id,
        int programId,
        string disApprovalRemarks,
        int actionedBy,
        int remarksId,
        CancellationToken cancellationToken)
    {
        await using var command =
            _context.Database.GetDbConnection().CreateCommand();

        command.CommandText = """
            SELECT public.uspDisApproveTransaction(
                @id,
                @programId,
                @disApprovalRemarks,
                @actionedBy,
                @remarksId
            )
            """;

        command.Parameters.Add(
            new NpgsqlParameter("id", id));

        command.Parameters.Add(
            new NpgsqlParameter("programId", programId));

        command.Parameters.Add(
            new NpgsqlParameter(
                "disApprovalRemarks",
                disApprovalRemarks));

        command.Parameters.Add(
            new NpgsqlParameter(
                "actionedBy",
                actionedBy));

        command.Parameters.Add(
            new NpgsqlParameter(
                "remarksId",
                remarksId));

        await _context.Database.OpenConnectionAsync(
            cancellationToken);

        await command.ExecuteScalarAsync(cancellationToken);
    }
}