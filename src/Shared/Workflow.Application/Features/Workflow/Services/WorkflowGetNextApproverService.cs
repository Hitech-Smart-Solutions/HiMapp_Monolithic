using Himapp.Workflow.Contracts.Models;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        await using var command =
            _context.Database.GetDbConnection().CreateCommand();

        command.CommandText = """
            SELECT *
            FROM public.uspgetnextapproverforcentralizedcommonworkflow(
                @projectId,
                @programId,
                @userId,
                @priority
            )
            """;

        command.Parameters.Add(
            new NpgsqlParameter("projectId", projectId));

        command.Parameters.Add(
            new NpgsqlParameter("programId", programId));

        command.Parameters.Add(
            new NpgsqlParameter("userId", userId));

        command.Parameters.Add(
            new NpgsqlParameter("priority", priority));

        await _context.Database.OpenConnectionAsync(
            cancellationToken);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new NextApproverModel(
            reader.GetInt32(reader.GetOrdinal("userid")),
            reader.GetString(reader.GetOrdinal("username")),
            reader.GetInt32(reader.GetOrdinal("workflowid")),
            reader.GetInt32(reader.GetOrdinal("workflowseq")),
            reader.GetInt32(reader.GetOrdinal("priority"))
        );
    }
}