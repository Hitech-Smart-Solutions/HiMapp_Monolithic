using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Services.IServices;
using Himapp.Execution.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Himapp.Execution.Application.Features.Planning.Services;

public sealed class PlanningSectionService : IPlanningSectionService
{
    private readonly IExecutionDbContext _db;
    public PlanningSectionService(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<PlanningSectionModel>> GetProjectSectionsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var result = new List<PlanningSectionModel>();

        try
        {
            var dbContext = _db as DbContext;

            if (dbContext is null)
            {
                throw new InvalidOperationException("IExecutionDbContext is not a DbContext.");
            }

            var conn = dbContext.Database.GetDbConnection();

            await conn.OpenAsync(cancellationToken);

            try
            {
                using var cmd = conn.CreateCommand();

                cmd.CommandText =
                    "SELECT sm.\"ID\", sm.\"SectionName\", smd.\"LabelName\" " +
                    "FROM \"SectionMaster\" sm " +
                    "INNER JOIN \"ProjectSectionMappingDetails\" psmd ON sm.\"ID\" = psmd.\"SectionID\" " +
                    "INNER JOIN \"ProjectSectionMapping\" smd ON smd.\"ID\" = psmd.\"ProjectSectionMappingID\" " +
                    "WHERE sm.\"IsActive\" = true AND smd.\"IsActive\" = true AND psmd.\"IsActive\" = true AND smd.\"ProjectID\" = @projectId";

                var p = cmd.CreateParameter();
                p.ParameterName = "@projectId";
                p.Value = projectId;
                cmd.Parameters.Add(p);

                using var rdr = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await rdr.ReadAsync(cancellationToken))
                {
                    var id = rdr.IsDBNull(0) ? 0 : rdr.GetInt32(0);
                    var sname = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                    var lname = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);
                    result.Add(new PlanningSectionModel(id, sname, lname));
                }
            }
            finally
            {
                await conn.CloseAsync();
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error while loading planning sections for ProjectId {projectId}.", ex);
        }
    }

}
