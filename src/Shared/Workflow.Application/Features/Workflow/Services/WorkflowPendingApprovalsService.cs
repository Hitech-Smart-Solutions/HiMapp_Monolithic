using Himapp.Workflow.Contracts.Models;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Himapp.Workflow.Application.Features.Workflow.Services;

public sealed class WorkflowPendingApprovalsService
    : IWorkflowPendingApprovalsService
{
    private readonly IWorkflowDbContext _context;

    public WorkflowPendingApprovalsService(IWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AwaitingDailyProgressModel>>
        GetAwaitingDailyProgress(
            int userId,
            CancellationToken cancellationToken)
    {
        var results = new List<AwaitingDailyProgressModel>();

        await using var connection =
            _context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT *
            FROM public."uspGetAwaitingDailyProgress"(@userId);
            """;

        command.Parameters.Add(
            new NpgsqlParameter("userId", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = userId
            });

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        var programRowIdOrdinal =
            reader.GetOrdinal("ProgramRowID");

        var entityIdOrdinal =
            reader.GetOrdinal("EntityID");

        var transactionCodeOrdinal =
            reader.GetOrdinal("TransactionCode");

        var transactionDateOrdinal =
            reader.GetOrdinal("TransactionDate");

        var programNameOrdinal =
            reader.GetOrdinal("ProgramName");

        var projectNameOrdinal =
            reader.GetOrdinal("ProjectName");

        var statusIdOrdinal =
            reader.GetOrdinal("StatusID");

        var statusNameOrdinal =
            reader.GetOrdinal("StatusName");

        var createdByOrdinal =
            reader.GetOrdinal("CreatedBy");

        var pendingApprovalForOrdinal =
            reader.GetOrdinal("PendingApprovalFor");

        var dprCodeOrdinal =
            reader.GetOrdinal("DPRCode");

        var projectIdOrdinal =
            reader.GetOrdinal("ProjectID");

        var isDisapproveOrdinal =
            reader.GetOrdinal("IsDisapprove");

        var isReferenceOrdinal =
            reader.GetOrdinal("IsReference");

        var approvalLevelOrdinal =
            reader.GetOrdinal("ApprovalLevel");

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new AwaitingDailyProgressModel
            {
                ProgramRowID =
                    reader.GetInt32(programRowIdOrdinal),

                EntityID =
                    reader.GetInt32(entityIdOrdinal),

                TransactionCode =
                    reader.IsDBNull(transactionCodeOrdinal)
                        ? null
                        : reader.GetString(transactionCodeOrdinal),

                TransactionDate =
                    reader.IsDBNull(transactionDateOrdinal)
                        ? null
                        : reader.GetDateTime(transactionDateOrdinal),

                ProgramName =
                    reader.IsDBNull(programNameOrdinal)
                        ? null
                        : reader.GetString(programNameOrdinal),

                ProjectName =
                    reader.IsDBNull(projectNameOrdinal)
                        ? null
                        : reader.GetString(projectNameOrdinal),

                StatusID =
                    reader.GetInt16(statusIdOrdinal),

                StatusName =
                    reader.IsDBNull(statusNameOrdinal)
                        ? null
                        : reader.GetString(statusNameOrdinal),

                CreatedBy =
                    reader.IsDBNull(createdByOrdinal)
                        ? null
                        : reader.GetString(createdByOrdinal),

                PendingApprovalFor =
                    reader.IsDBNull(pendingApprovalForOrdinal)
                        ? null
                        : reader.GetString(pendingApprovalForOrdinal),

                DPRCode =
                    reader.IsDBNull(dprCodeOrdinal)
                        ? null
                        : reader.GetString(dprCodeOrdinal),

                ProjectID =
                    reader.GetInt32(projectIdOrdinal),

                IsDisapprove =
                    reader.GetInt64(isDisapproveOrdinal),

                IsReference =
                    reader.GetBoolean(isReferenceOrdinal),

                ApprovalLevel =
                    reader.GetInt16(approvalLevelOrdinal)
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<AwaitingDepartmentalLabourSlipModel>>
    GetAwaitingDepartmentalLabourSlip(
        int userId,
        CancellationToken cancellationToken)
    {
        var result = new List<AwaitingDepartmentalLabourSlipModel>();

        await using var connection =
            _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT *
        FROM public."uspGetAwaitingDepartmentalLabourSlip"(@userId);
        """;

        command.Parameters.Add(
            new NpgsqlParameter("userId", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = userId
            });

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new AwaitingDepartmentalLabourSlipModel
            {
                ProgramRowID =
                    reader.GetInt32(reader.GetOrdinal("ProgramRowID")),

                EntityID =
                    reader.GetInt32(reader.GetOrdinal("EntityID")),

                TransactionCode =
                    reader["TransactionCode"] as string,

                TransactionDate =
                    reader.IsDBNull(reader.GetOrdinal("TransactionDate"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("TransactionDate")),

                ProgramName =
                    reader["ProgramName"] as string,

                ProjectName =
                    reader["ProjectName"] as string,

                StatusID =
                    reader.GetInt16(reader.GetOrdinal("StatusID")),

                StatusName =
                    reader["StatusName"] as string,

                CreatedBy =
                    reader["CreatedBy"] as string,

                PendingApprovalFor =
                    reader["PendingApprovalFor"] as string,

                SlipNo =
                    reader["SlipNo"] as string,

                PartyName =
                    reader["PartyName"] as string,

                PartyID =
                    reader.IsDBNull(reader.GetOrdinal("PartyID"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("PartyID")),

                ProjectID =
                    reader.GetInt32(reader.GetOrdinal("ProjectID")),

                DocumentName =
                    reader["DocumentName"] as string,

                DocumentPath =
                    reader["DocumentPath"] as string,

                DocumentContentType =
                    reader["DocumentContentType"] as string,

                IsDisapprove =
                    reader.GetInt64(
                        reader.GetOrdinal("IsDisapprove")),

                IsReference =
                    reader.GetBoolean(
                        reader.GetOrdinal("IsReference")),

                ApprovalLevel =
                    reader.GetInt16(
                        reader.GetOrdinal("ApprovalLevel"))
            });
        }

        return result;
    }
}