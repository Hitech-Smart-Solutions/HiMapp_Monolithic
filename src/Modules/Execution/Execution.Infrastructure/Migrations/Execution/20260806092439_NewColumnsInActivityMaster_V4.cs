using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class NewColumnsInActivityMaster_V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OtherLabourRate",
                schema: "execution",
                table: "ProjectActivities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "OutputRequired",
                schema: "execution",
                table: "ProjectActivities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SkilledLabourRate",
                schema: "execution",
                table: "ProjectActivities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnSkilledLabourRate",
                schema: "execution",
                table: "ProjectActivities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherLabourRate",
                schema: "execution",
                table: "Activities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "OutputRequired",
                schema: "execution",
                table: "Activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SkilledLabourRate",
                schema: "execution",
                table: "Activities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnSkilledLabourRate",
                schema: "execution",
                table: "Activities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherLabourRate",
                schema: "execution",
                table: "ProjectActivities");

            migrationBuilder.DropColumn(
                name: "OutputRequired",
                schema: "execution",
                table: "ProjectActivities");

            migrationBuilder.DropColumn(
                name: "SkilledLabourRate",
                schema: "execution",
                table: "ProjectActivities");

            migrationBuilder.DropColumn(
                name: "UnSkilledLabourRate",
                schema: "execution",
                table: "ProjectActivities");

            migrationBuilder.DropColumn(
                name: "OtherLabourRate",
                schema: "execution",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "OutputRequired",
                schema: "execution",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SkilledLabourRate",
                schema: "execution",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UnSkilledLabourRate",
                schema: "execution",
                table: "Activities");
        }
    }
}
