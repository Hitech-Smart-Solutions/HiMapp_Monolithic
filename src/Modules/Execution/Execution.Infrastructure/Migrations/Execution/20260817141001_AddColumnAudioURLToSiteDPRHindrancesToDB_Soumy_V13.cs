using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class AddColumnAudioURLToSiteDPRHindrancesToDB_Soumy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HindranceAudioUrl",
                schema: "execution",
                table: "SiteDailyProgresses");

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                schema: "execution",
                table: "SiteDailyProgressHindrance",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUrl",
                schema: "execution",
                table: "SiteDailyProgressHindrance");

            migrationBuilder.AddColumn<string>(
                name: "HindranceAudioUrl",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "text",
                nullable: true);
        }
    }
}
