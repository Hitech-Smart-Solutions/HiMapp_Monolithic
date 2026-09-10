using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_UpdateColumnStateIDToStatusIDDDLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hindrance",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AudioUrl",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NextDayPlan",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                schema: "execution",
                table: "SiteDailyProgressDetail",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hindrance",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AudioUrl",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NextDayPlan",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                schema: "execution",
                table: "SiteDailyProgressDetail",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "StateID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "smallint",
                nullable: true);
        }
    }
}
