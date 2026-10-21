using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class InitialPmMigration_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PAndM");

            // Move existing tables from "PM" to "PAndM"
            migrationBuilder.RenameTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PM",
                newName: "EquipmentPreventiveMaintenancePoints",
                newSchema: "PAndM");

            migrationBuilder.RenameTable(
                name: "MeterReadingType",
                schema: "PM",
                newName: "MeterReadingType",
                newSchema: "PAndM");

            migrationBuilder.RenameTable(
                name: "PreventiveMaintenancePoint",
                schema: "PM",
                newName: "PreventiveMaintenancePoint",
                newSchema: "PAndM");

            // Move existing tables from "pm" to "PAndM"
            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "pm",
                newName: "DocumentTypes",
                newSchema: "PAndM");

            migrationBuilder.RenameTable(
                name: "WorkDescriptions",
                schema: "pm",
                newName: "WorkDescriptions",
                newSchema: "PAndM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PM");

            migrationBuilder.EnsureSchema(
                name: "pm");

            migrationBuilder.RenameTable(
                name: "WorkDescriptions",
                schema: "PAndM",
                newName: "WorkDescriptions",
                newSchema: "pm");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "PAndM",
                newName: "DocumentTypes",
                newSchema: "pm");

            migrationBuilder.RenameTable(
                name: "PreventiveMaintenancePoint",
                schema: "PAndM",
                newName: "PreventiveMaintenancePoint",
                newSchema: "PM");

            migrationBuilder.RenameTable(
                name: "MeterReadingType",
                schema: "PAndM",
                newName: "MeterReadingType",
                newSchema: "PM");

            migrationBuilder.RenameTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PAndM",
                newName: "EquipmentPreventiveMaintenancePoints",
                newSchema: "PM");
        }
    }
}
