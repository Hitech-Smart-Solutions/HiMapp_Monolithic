using Himapp.Workflow.Contracts.Models;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

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
        int programId,
        int statusId,
        string? remarks,
        int actionedBy,
        int nextApproverId,
        int priority,
        CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT *
            FROM public."uspChangeApprovalForCentralizedCommonWorkFlow"(
                @id,
                @projectId,
                @programId,
                @statusId,
                @remarks,
                @actionedBy,
                @nextApproverId,
                @priority
            )
            """;

        command.Parameters.Add(new NpgsqlParameter("id", id));
        command.Parameters.Add(new NpgsqlParameter("projectId", projectId));
        command.Parameters.Add(new NpgsqlParameter("programId", programId));
        command.Parameters.Add(new NpgsqlParameter("statusId", statusId));
        command.Parameters.Add(
            new NpgsqlParameter(
                "remarks",
                (object?)remarks ?? DBNull.Value));
        command.Parameters.Add(
            new NpgsqlParameter("actionedBy", actionedBy));
        command.Parameters.Add(
            new NpgsqlParameter("nextApproverId", nextApproverId));
        command.Parameters.Add(
            new NpgsqlParameter("priority", priority));

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is null || result == DBNull.Value)
        {
            return null;
        }

        return new ChangeApprovalModel(
            Convert.ToInt32(result),
            id,
            statusId,
            nextApproverId
        );
    }
}