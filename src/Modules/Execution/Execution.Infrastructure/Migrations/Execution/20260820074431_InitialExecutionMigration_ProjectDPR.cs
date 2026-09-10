using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration_ProjectDPR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyProgressDetails_DailyProgresses_DailyProgressID",
                schema: "execution",
                table: "DailyProgressDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyProgressPhoto_DailyProgresses_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyProgressPhoto",
                schema: "execution",
                table: "DailyProgressPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyProgresses",
                schema: "execution",
                table: "DailyProgresses");

            migrationBuilder.DropColumn(
                name: "Uom",
                schema: "execution",
                table: "DailyProgressDetails");

            migrationBuilder.DropColumn(
                name: "HindranceAudioUrl",
                schema: "execution",
                table: "DailyProgresses");

            migrationBuilder.DropColumn(
                name: "Hindrances",
                schema: "execution",
                table: "DailyProgresses");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "execution",
                table: "DailyProgresses");

            migrationBuilder.RenameTable(
                name: "DailyProgressPhoto",
                schema: "execution",
                newName: "DailyProgressPhotos",
                newSchema: "execution");

            migrationBuilder.RenameTable(
                name: "DailyProgresses",
                schema: "execution",
                newName: "DailyProgress",
                newSchema: "execution");

            migrationBuilder.RenameIndex(
                name: "IX_DailyProgressPhoto_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhotos",
                newName: "IX_DailyProgressPhotos_DailyProgressID");

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Variance",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlanQuantity",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "UOMID",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgress",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "execution",
                table: "DailyProgress",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgress",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                schema: "execution",
                table: "DailyProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyProgressPhotos",
                schema: "execution",
                table: "DailyProgressPhotos",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyProgress",
                schema: "execution",
                table: "DailyProgress",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "DailyProgressHindrances",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DailyProgressID = table.Column<int>(type: "integer", nullable: false),
                    Hindrance = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProgressHindrances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DailyProgressHindrances_DailyProgress_DailyProgressID",
                        column: x => x.DailyProgressID,
                        principalSchema: "execution",
                        principalTable: "DailyProgress",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyProgressHindrances_DailyProgressID",
                schema: "execution",
                table: "DailyProgressHindrances",
                column: "DailyProgressID");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyProgressDetails_DailyProgress_DailyProgressID",
                schema: "execution",
                table: "DailyProgressDetails",
                column: "DailyProgressID",
                principalSchema: "execution",
                principalTable: "DailyProgress",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyProgressPhotos_DailyProgress_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhotos",
                column: "DailyProgressID",
                principalSchema: "execution",
                principalTable: "DailyProgress",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyProgressDetails_DailyProgress_DailyProgressID",
                schema: "execution",
                table: "DailyProgressDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyProgressPhotos_DailyProgress_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropTable(
                name: "DailyProgressHindrances",
                schema: "execution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyProgressPhotos",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyProgress",
                schema: "execution",
                table: "DailyProgress");

            migrationBuilder.DropColumn(
                name: "UOMID",
                schema: "execution",
                table: "DailyProgressDetails");

            migrationBuilder.DropColumn(
                name: "StatusID",
                schema: "execution",
                table: "DailyProgress");

            migrationBuilder.RenameTable(
                name: "DailyProgressPhotos",
                schema: "execution",
                newName: "DailyProgressPhoto",
                newSchema: "execution");

            migrationBuilder.RenameTable(
                name: "DailyProgress",
                schema: "execution",
                newName: "DailyProgresses",
                newSchema: "execution");

            migrationBuilder.RenameIndex(
                name: "IX_DailyProgressPhotos_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhoto",
                newName: "IX_DailyProgressPhoto_DailyProgressID");

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Variance",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlanQuantity",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                schema: "execution",
                table: "DailyProgressDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgressPhoto",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgressPhoto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgressPhoto",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgressPhoto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgresses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "execution",
                table: "DailyProgresses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "LastModifiedBy",
                schema: "execution",
                table: "DailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "execution",
                table: "DailyProgresses",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "execution",
                table: "DailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "HindranceAudioUrl",
                schema: "execution",
                table: "DailyProgresses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hindrances",
                schema: "execution",
                table: "DailyProgresses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "execution",
                table: "DailyProgresses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyProgressPhoto",
                schema: "execution",
                table: "DailyProgressPhoto",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyProgresses",
                schema: "execution",
                table: "DailyProgresses",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyProgressDetails_DailyProgresses_DailyProgressID",
                schema: "execution",
                table: "DailyProgressDetails",
                column: "DailyProgressID",
                principalSchema: "execution",
                principalTable: "DailyProgresses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyProgressPhoto_DailyProgresses_DailyProgressID",
                schema: "execution",
                table: "DailyProgressPhoto",
                column: "DailyProgressID",
                principalSchema: "execution",
                principalTable: "DailyProgresses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
