using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class newMigrationAfterColumnChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueId",
                schema: "execution",
                table: "DailyProgresses",
                newName: "UniqueID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "execution",
                table: "DailyProgresses",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "execution",
                table: "DailyProgresses",
                newName: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueID",
                schema: "execution",
                table: "DailyProgresses",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                schema: "execution",
                table: "DailyProgresses",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "execution",
                table: "DailyProgresses",
                newName: "Id");
        }
    }
}
