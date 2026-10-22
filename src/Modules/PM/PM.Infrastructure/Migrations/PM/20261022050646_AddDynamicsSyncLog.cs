using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class AddDynamicsSyncLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicsSyncLog",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SyncMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TriggerSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JobId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordsFetched = table.Column<int>(type: "integer", nullable: false),
                    RecordsInserted = table.Column<int>(type: "integer", nullable: false),
                    RecordsUpdated = table.Column<int>(type: "integer", nullable: false),
                    RecordsReset = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    ErrorDetails = table.Column<string>(type: "text", nullable: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicsSyncLog", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicsSyncLog_EntityName_Status",
                schema: "PAndM",
                table: "DynamicsSyncLog",
                columns: new[] { "EntityName", "Status" });

            // List functions for GET sync-logs (paging, search, filter on one column, sort).
            // Uses the helper functions created in AddEquipmentListFunctions.
            migrationBuilder.Sql("""
CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicssynclogs(
    p_filtercolumn  text    DEFAULT NULL,
    p_filtervalue   text    DEFAULT NULL,
    p_pageindex     integer DEFAULT 1,
    p_pagesize      integer DEFAULT 25,
    p_sortcolumn    text    DEFAULT NULL,
    p_sortdirection text    DEFAULT 'asc')
RETURNS SETOF "PAndM"."DynamicsSyncLog"
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table   regclass := '"PAndM"."DynamicsSyncLog"'::regclass;
    v_where   text;
    v_sortcol text;
    v_dir     text;
    v_size    integer;
    v_page    integer;
BEGIN
    v_where   := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['EntityName','SyncMode','TriggerSource','Status','Message']);
    v_sortcol := COALESCE("PAndM".fn_resolve_column(v_table, p_sortcolumn), 'ID');
    v_dir     := CASE WHEN lower(btrim(COALESCE(p_sortdirection, ''))) = 'desc' THEN 'DESC' ELSE 'ASC' END;
    v_size    := LEAST(GREATEST(COALESCE(p_pagesize, 25), 1), 200);
    v_page    := GREATEST(COALESCE(p_pageindex, 1), 1);

    RETURN QUERY EXECUTE format(
        'SELECT t.* FROM %s t %s ORDER BY t.%I %s, t."ID" LIMIT %s OFFSET %s',
        v_table, v_where, v_sortcol, v_dir, v_size, (v_page - 1) * v_size)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;
""");

            migrationBuilder.Sql("""
CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicssynclogscount(
    p_filtercolumn  text DEFAULT NULL,
    p_filtervalue   text DEFAULT NULL)
RETURNS TABLE (cnt bigint)
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table regclass := '"PAndM"."DynamicsSyncLog"'::regclass;
    v_where text;
BEGIN
    v_where := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['EntityName','SyncMode','TriggerSource','Status','Message']);

    RETURN QUERY EXECUTE format('SELECT count(*)::bigint FROM %s t %s', v_table, v_where)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicssynclogscount(text, text);
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicssynclogs(text, text, integer, integer, text, text);
""");

            migrationBuilder.DropTable(
                name: "DynamicsSyncLog",
                schema: "PAndM");
        }
    }
}
