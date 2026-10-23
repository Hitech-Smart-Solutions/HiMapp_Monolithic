
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;

namespace Himapp.PM.Application.Features.Common;

internal static class PagedStoredProcedureQueryExecutor
{
    public static async Task<PagedResult<T>> ExecuteAsync<T>(
        IPMDbContext db,
        string dataFunction,
        string countFunction,
        int page,
        int pageSize,
        string? filterValue,
        string? sortColumn,
        string? isActive,
        Func<DbDataReader, T> map,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var pageIndex = page - 1;

        if (db is not DbContext dbContext)
        {
            throw new InvalidOperationException(
                "IPMDbContext is not a DbContext. Cannot obtain a database connection.");
        }

        var items = new List<T>();
        long totalCount;

        var connectionString =
            dbContext.Database.GetDbConnection().ConnectionString;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using (var countCommand = CreateCommand(
            connection,
            countFunction,
            pageIndex,
            pageSize,
            filterValue,
            sortColumn,
            isActive))
        {
            var countResult =
                await countCommand.ExecuteScalarAsync(cancellationToken);

            totalCount = countResult is null or DBNull
                ? 0
                : Convert.ToInt64(countResult);
        }

        await using (var dataCommand = CreateCommand(
            connection,
            dataFunction,
            pageIndex,
            pageSize,
            filterValue,
            sortColumn,
            isActive))
        await using (var reader =
            await dataCommand.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(map(reader));
            }
        }

        return new PagedResult<T>(
            items,
            page,
            pageSize,
            checked((int)totalCount));
    }

    private static NpgsqlCommand CreateCommand(
        NpgsqlConnection connection,
        string functionName,
        int pageIndex,
        int pageSize,
        string? filterValue,
        string? sortColumn,
        string? isActive)
    {
        // Cast string arguments to varchar to match PostgreSQL function signatures.
        var isCountFunction = functionName.EndsWith(
            "count",
            StringComparison.OrdinalIgnoreCase);

        var sql = $"""
            SELECT {(isCountFunction ? "cnt" : "*")}
            FROM "PAndM".{functionName}(
                CAST(@p_filtervalue AS varchar),
                @p_pageindex,
                @p_pagesize,
                CAST(@p_sortcolumn AS varchar),
                CAST(@p_isactive AS varchar)
            )
            """;

        var command = new NpgsqlCommand(sql, connection)
        {
            CommandType = CommandType.Text,
            CommandTimeout = isCountFunction ? 10 : 30
        };

        command.Parameters.AddWithValue(
            "@p_filtervalue",
            NpgsqlDbType.Text,
            ValueOrNull(filterValue));

        command.Parameters.AddWithValue(
            "@p_pageindex",
            NpgsqlDbType.Integer,
            pageIndex);

        command.Parameters.AddWithValue(
            "@p_pagesize",
            NpgsqlDbType.Integer,
            pageSize);

        command.Parameters.AddWithValue(
            "@p_sortcolumn",
            NpgsqlDbType.Text,
            ValueOrNull(sortColumn));

        command.Parameters.AddWithValue(
            "@p_isactive",
            NpgsqlDbType.Text,
            string.IsNullOrWhiteSpace(isActive)
                ? "all"
                : isActive.Trim());

        return command;
    }

    private static object ValueOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? DBNull.Value
            : value;
}
