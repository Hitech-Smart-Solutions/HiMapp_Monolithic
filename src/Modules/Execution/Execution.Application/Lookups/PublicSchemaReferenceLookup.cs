using Himapp.Execution.Contracts.References;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Himapp.Execution.Application.Lookups;

/// <summary>
/// Reads ProjectMaster data from the public schema using the application's DB connection.
/// This adapter is pragmatic for environments where ProjectMaster lives in the same DB
/// but under the public schema.
/// </summary>
internal sealed class PublicSchemaReferenceLookup : IReferenceLookupService
{
    private readonly IExecutionDbContext _db;

    public PublicSchemaReferenceLookup(IExecutionDbContext db) => _db = db;

    public async Task<ProjectMasterDto?> GetProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        var conn = _db.Database.GetDbConnection();
        await EnsureOpenAsync(conn, cancellationToken);

        var tableCandidates = new[] { "ProjectMaster", "ProjectMaster" };

        foreach (var table in tableCandidates)
        {
            var cmd = conn.CreateCommand();
            // Try to match different column name casings used by various projects (Id vs ID)
            cmd.CommandText = $"SELECT * FROM public.\"{table}\" WHERE (\"ID\" = @id) LIMIT 1";
            var p = cmd.CreateParameter();
            p.ParameterName = "@id";
            p.Value = id;
            cmd.Parameters.Add(p);

            try
            {
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    static T ReadOr<T>(DbDataReader r, string[] names, T @default)
                    {
                        foreach (var name in names)
                        {
                            try
                            {
                                var idx = r.GetOrdinal(name);
                                if (r.IsDBNull(idx)) return @default;
                                return r.GetFieldValue<T>(idx);
                            }
                            catch { }
                        }
                        return @default;
                    }

                    static T? ReadNullable<T>(DbDataReader r, string[] names)
                    {
                        foreach (var name in names)
                        {
                            try
                            {
                                var idx = r.GetOrdinal(name);
                                if (r.IsDBNull(idx)) return default;
                                return r.GetFieldValue<T>(idx);
                            }
                            catch { }
                        }
                        return default;
                    }

                    var dto = new ProjectMasterDto(
                        ReadOr(reader, new[] { "UniqueId", "UniqueID", "uniqueid" }, System.Guid.Empty),
                        ReadOr(reader, new[] { "Id", "ID", "id" }, 0),
                        ReadOr(reader, new[] { "ProjectName", "Projectname", "projectname" }, string.Empty),
                        ReadOr(reader, new[] { "ProjectCode", "Projectcode", "projectcode" }, string.Empty),
                        ReadOr(reader, new[] { "ClientName", "Clientname", "clientname" }, string.Empty),
                        ReadOr(reader, new[] { "ProjectValue", "Projectvalue", "projectvalue" }, 0m),
                        ReadOr(reader, new[] { "CompanyId", "CompanyID", "companyid" }, 0),
                        ReadOr(reader, new[] { "LocationId", "LocationID", "locationid" }, 0),
                        ReadNullable<string>(reader, new[] { "Address", "address" }),
                        ReadNullable<string>(reader, new[] { "ProjectDescription", "Projectdescription", "projectdescription" }),
                        ReadNullable<string>(reader, new[] { "Website", "website" }),
                        ReadOr(reader, new[] { "ContactNumber", "Contactnumber", "contactnumber" }, string.Empty),
                        ReadNullable<string>(reader, new[] { "EmailId", "EmailID", "emailid" }),
                        ReadNullable<int>(reader, new[] { "ProjectTypeId", "ProjectTypeID", "projecttypeid" }),
                        ReadNullable<System.DateTime>(reader, new[] { "StartDate", "startdate" }),
                        ReadNullable<System.DateTime>(reader, new[] { "EndDate", "enddate" }),
                        ReadNullable<int>(reader, new[] { "ProjectHeadID", "ProjectHeadId", "projectheadid" }),
                        ReadNullable<int>(reader, new[] { "SalesManagerID", "SalesManagerId", "salesmanagerid" }),
                        ReadNullable<int>(reader, new[] { "StoreInchargeID", "StoreInchargeId", "storeinchargeid" }),
                        ReadNullable<int>(reader, new[] { "ProjectManagerID", "ProjectManagerId", "projectmanagerid" }),
                        ReadNullable<int>(reader, new[] { "ContractTypeID", "ContractTypeId", "contracttypeid" }),
                        ReadOr(reader, new[] { "StatusId", "StatusID", "statusid" }, 0),
                        ReadOr(reader, new[] { "IsActive", "Isactive", "isactive" }, false),
                        ReadOr(reader, new[] { "CreatedBy", "Createdby", "createdby" }, 0),
                        ReadOr(reader, new[] { "CreatedDate", "Createddate", "createddate" }, System.DateTime.MinValue),
                        ReadOr(reader, new[] { "LastModifiedBy", "LastModifiedby", "lastmodifiedby" }, 0),
                        ReadOr(reader, new[] { "LastModifiedDate", "LastModifieddate", "lastmodifieddate" }, System.DateTime.MinValue)
                    );

                    return dto;
                }
            }
            catch
            {
                // try next candidate
            }
        }

        return null;
    }

    public async Task<IEnumerable<ProjectMasterDto>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var conn = _db.Database.GetDbConnection();
        await EnsureOpenAsync(conn, cancellationToken);

        var list = new List<ProjectMasterDto>();
        var tableCandidates = new[] { "ProjectMasters", "ProjectMaster" };

        foreach (var table in tableCandidates)
        {
            var cmd = conn.CreateCommand();
            // For listing all projects use unqualified select all; field reading is resilient to name casing
            cmd.CommandText = $"SELECT * FROM public.\"{table}\"";

            try
            {
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    static T ReadOr<T>(DbDataReader r, string name, T @default)
                    {
                        try
                        {
                            var idx = r.GetOrdinal(name);
                            if (r.IsDBNull(idx)) return @default;
                            return r.GetFieldValue<T>(idx);
                        }
                        catch
                        {
                            return @default;
                        }
                    }

                    static T? ReadNullable<T>(DbDataReader r, string name)
                    {
                        try
                        {
                            var idx = r.GetOrdinal(name);
                            if (r.IsDBNull(idx)) return default;
                            return r.GetFieldValue<T>(idx);
                        }
                        catch
                        {
                            return default;
                        }
                    }

                    var dto = new ProjectMasterDto(
                        ReadOr(reader, "UniqueId", System.Guid.Empty),
                        ReadOr(reader, "Id", 0),
                        ReadOr(reader, "ProjectName", string.Empty),
                        ReadOr(reader, "ProjectCode", string.Empty),
                        ReadOr(reader, "ClientName", string.Empty),
                        ReadOr(reader, "ProjectValue", 0m),
                        ReadOr(reader, "CompanyId", 0),
                        ReadOr(reader, "LocationId", 0),
                        ReadNullable<string>(reader, "Address"),
                        ReadNullable<string>(reader, "ProjectDescription"),
                        ReadNullable<string>(reader, "Website"),
                        ReadOr(reader, "ContactNumber", string.Empty),
                        ReadNullable<string>(reader, "EmailId"),
                        ReadNullable<int>(reader, "ProjectTypeId"),
                        ReadNullable<System.DateTime>(reader, "StartDate"),
                        ReadNullable<System.DateTime>(reader, "EndDate"),
                        ReadNullable<int>(reader, "ProjectHeadID"),
                        ReadNullable<int>(reader, "SalesManagerID"),
                        ReadNullable<int>(reader, "StoreInchargeID"),
                        ReadNullable<int>(reader, "ProjectManagerID"),
                        ReadNullable<int>(reader, "ContractTypeID"),
                        ReadOr(reader, "StatusId", 0),
                        ReadOr(reader, "IsActive", false),
                        ReadOr(reader, "CreatedBy", 0),
                        ReadOr(reader, "CreatedDate", System.DateTime.MinValue),
                        ReadOr(reader, "LastModifiedBy", 0),
                        ReadOr(reader, "LastModifiedDate", System.DateTime.MinValue)
                    );

                    list.Add(dto);
                }

                if (list.Count > 0) break;
            }
            catch
            {
                // try next candidate
            }
        }

        return list;
    }

    public Task<UomDto?> GetUomAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<UomDto?>(null);
    }

    public Task<IEnumerable<UomDto>> GetUomsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<UomDto>>(Array.Empty<UomDto>());
    }

    public Task<ProjectLocationMasterDto?> GetProjectLocationAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProjectLocationMasterDto?>(null);
    }

    public Task<IEnumerable<ProjectLocationMasterDto>> GetProjectLocationsByProjectIdAsync(long projectId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ProjectLocationMasterDto>>(Array.Empty<ProjectLocationMasterDto>());
    }

    private static async Task EnsureOpenAsync(DbConnection conn, CancellationToken cancellationToken)
    {
        if (conn.State != System.Data.ConnectionState.Open)
        {
            await conn.OpenAsync(cancellationToken);
        }
    }
}
