using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class AddSiteDPRHindrancesToDB_Soumy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hindrances",
                schema: "execution",
                table: "SiteDailyProgresses");

            migrationBuilder.CreateTable(
                name: "SiteDailyProgressHindrance",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyProgressID = table.Column<int>(type: "integer", nullable: false),
                    Hindrance = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDailyProgressHindrance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SiteDailyProgressHindrance_SiteDailyProgresses_DailyProgres~",
                        column: x => x.DailyProgressID,
                        principalSchema: "execution",
                        principalTable: "SiteDailyProgresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteDailyProgressHindrance_DailyProgressID",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                column: "DailyProgressID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteDailyProgressHindrance",
                schema: "execution");

            migrationBuilder.AddColumn<string>(
                name: "Hindrances",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "text",
                nullable: true);
        }
    }
}
