using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_AddFileNameFileSixeFileTypeInSiteDPRPhotoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileSize",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                schema: "execution",
                table: "SiteDailyProgressPhoto",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                schema: "execution",
                table: "SiteDailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "execution",
                table: "SiteDailyProgressPhoto");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "execution",
                table: "SiteDailyProgressPhoto");

            migrationBuilder.DropColumn(
                name: "FileType",
                schema: "execution",
                table: "SiteDailyProgressPhoto");

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
        }
    }
}
