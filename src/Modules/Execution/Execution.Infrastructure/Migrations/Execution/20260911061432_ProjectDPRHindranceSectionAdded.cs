using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class ProjectDPRHindranceSectionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionID",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionID",
                schema: "execution",
                table: "DailyProgressHindrances",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionID",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropColumn(
                name: "SectionID",
                schema: "execution",
                table: "DailyProgressHindrances");
        }
    }
}
