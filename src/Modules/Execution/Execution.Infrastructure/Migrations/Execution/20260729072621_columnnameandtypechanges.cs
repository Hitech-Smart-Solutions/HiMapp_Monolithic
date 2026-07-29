using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class columnnameandtypechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uom",
                schema: "execution",
                table: "RateMasters");

            migrationBuilder.DropColumn(
                name: "Uom",
                schema: "execution",
                table: "PlanningDetails");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "RateMasters",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "execution",
                table: "RateMasters",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                schema: "execution",
                table: "RateMasters",
                newName: "ActivityID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "RateMasters",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "ProjectActivities",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "execution",
                table: "ProjectActivities",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                schema: "execution",
                table: "ProjectActivities",
                newName: "ActivityID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "ProjectActivities",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "Plannings",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "execution",
                table: "Plannings",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "Plannings",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "PlanningDetails",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "PlanningId",
                schema: "execution",
                table: "PlanningDetails",
                newName: "PlanningID");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                schema: "execution",
                table: "PlanningDetails",
                newName: "AreaID");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                schema: "execution",
                table: "PlanningDetails",
                newName: "ActivityID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "PlanningDetails",
                newName: "ID");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityID",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UOMID",
                schema: "execution",
                table: "RateMasters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                schema: "execution",
                table: "ProjectActivities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "ProjectActivities",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "ProjectActivities",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityID",
                schema: "execution",
                table: "ProjectActivities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                schema: "execution",
                table: "ProjectActivities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PlanningID",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaID",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityID",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UOMID",
                schema: "execution",
                table: "PlanningDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UOMID",
                schema: "execution",
                table: "RateMasters");

            migrationBuilder.DropColumn(
                name: "UOMID",
                schema: "execution",
                table: "PlanningDetails");

            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "SiteDailyProgresses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "RateMasters",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                schema: "execution",
                table: "RateMasters",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ActivityID",
                schema: "execution",
                table: "RateMasters",
                newName: "ActivityId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "RateMasters",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "ProjectActivities",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                schema: "execution",
                table: "ProjectActivities",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ActivityID",
                schema: "execution",
                table: "ProjectActivities",
                newName: "ActivityId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "ProjectActivities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "Plannings",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                schema: "execution",
                table: "Plannings",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "Plannings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "PlanningDetails",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "PlanningID",
                schema: "execution",
                table: "PlanningDetails",
                newName: "PlanningId");

            migrationBuilder.RenameColumn(
                name: "AreaID",
                schema: "execution",
                table: "PlanningDetails",
                newName: "AreaId");

            migrationBuilder.RenameColumn(
                name: "ActivityID",
                schema: "execution",
                table: "PlanningDetails",
                newName: "ActivityId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "PlanningDetails",
                newName: "Id");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                schema: "execution",
                table: "RateMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "RateMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                schema: "execution",
                table: "RateMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ActivityId",
                schema: "execution",
                table: "RateMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "execution",
                table: "RateMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                schema: "execution",
                table: "RateMasters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                schema: "execution",
                table: "ProjectActivities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "ProjectActivities",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                schema: "execution",
                table: "ProjectActivities",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ActivityId",
                schema: "execution",
                table: "ProjectActivities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "execution",
                table: "ProjectActivities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                schema: "execution",
                table: "Plannings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "Plannings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                schema: "execution",
                table: "Plannings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "execution",
                table: "Plannings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "PlanningId",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AreaId",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "ActivityId",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "execution",
                table: "PlanningDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                schema: "execution",
                table: "PlanningDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
