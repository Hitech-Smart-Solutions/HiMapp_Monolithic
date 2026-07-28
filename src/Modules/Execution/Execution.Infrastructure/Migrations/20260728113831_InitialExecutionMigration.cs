using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyID = table.Column<int>(type: "integer", nullable: false),
                    ActivityName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DailyDepartmentalLabourSlips",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyID = table.Column<int>(type: "integer", nullable: true),
                    LocationID = table.Column<int>(type: "integer", nullable: true),
                    ProjectID = table.Column<int>(type: "integer", nullable: true),
                    FinancialYearID = table.Column<int>(type: "integer", nullable: true),
                    DDLSlipCode = table.Column<string>(type: "text", nullable: true),
                    SlipDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SlipNumber = table.Column<int>(type: "integer", nullable: true),
                    PartyID = table.Column<int>(type: "integer", nullable: true),
                    IsNewParty = table.Column<bool>(type: "boolean", nullable: true),
                    NewParty = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    StateID = table.Column<short>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentName = table.Column<string>(type: "text", nullable: true),
                    DocumentContentType = table.Column<string>(type: "text", nullable: true),
                    DocumentPath = table.Column<string>(type: "text", nullable: true),
                    IsDisapproved = table.Column<int>(type: "integer", nullable: true),
                    TotalWrkMins = table.Column<int>(type: "integer", nullable: true),
                    DPRSlipIssueID = table.Column<int>(type: "integer", nullable: true),
                    TotalDPRManpower = table.Column<int>(type: "integer", nullable: true),
                    Skilled = table.Column<int>(type: "integer", nullable: true),
                    UnSkilled = table.Column<int>(type: "integer", nullable: true),
                    Mat = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyDepartmentalLabourSlips", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DailyLabor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    DLRCode = table.Column<string>(type: "text", nullable: true),
                    ReportDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConstraintsAndReasons = table.Column<string>(type: "text", nullable: true),
                    ProposedActionPlan = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CompanyID = table.Column<int>(type: "integer", nullable: true),
                    ProjectID = table.Column<int>(type: "integer", nullable: true),
                    StateID = table.Column<short>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemoveMenPower = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLabor", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DailyProgressDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyProgressID = table.Column<int>(type: "integer", nullable: false),
                    ActivityID = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Uom = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PlanQuantity = table.Column<decimal>(type: "numeric", nullable: true),
                    Variance = table.Column<decimal>(type: "numeric", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProgressDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DailyProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Hindrances = table.Column<string>(type: "text", nullable: true),
                    HindranceAudioUrl = table.Column<string>(type: "text", nullable: true),
                    NextDayPlan = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProgresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manpowers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    SectionID = table.Column<int>(type: "integer", nullable: false),
                    EntryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    StateID = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manpowers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PlanningDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanningId = table.Column<long>(type: "bigint", nullable: false),
                    AreaId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityId = table.Column<long>(type: "bigint", nullable: false),
                    TargetQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Uom = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plannings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    PlanType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityId = table.Column<long>(type: "bigint", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RateMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityId = table.Column<long>(type: "bigint", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    Uom = table.Column<string>(type: "text", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteDailyProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SectionID = table.Column<int>(type: "integer", nullable: false),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Hindrances = table.Column<string>(type: "text", nullable: true),
                    HindranceAudioUrl = table.Column<string>(type: "text", nullable: true),
                    NextDayPlan = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDailyProgresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyDepartmentalLabourSlipDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    DDLSlipID = table.Column<int>(type: "integer", nullable: true),
                    LabourCategoryTypeID = table.Column<int>(type: "integer", nullable: true),
                    NumOfLabour = table.Column<int>(type: "integer", nullable: true),
                    FromTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TOTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LunchHour = table.Column<decimal>(type: "numeric", nullable: true),
                    WorkingHours = table.Column<decimal>(type: "numeric", nullable: true),
                    WorkLocationID = table.Column<int>(type: "integer", nullable: true),
                    ActivityCategoryID = table.Column<int>(type: "integer", nullable: true),
                    ActivityDetails = table.Column<string>(type: "text", nullable: true),
                    UOMID = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    DebitPartyID = table.Column<int>(type: "integer", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    StateID = table.Column<short>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsLumSumWork = table.Column<bool>(type: "boolean", nullable: true),
                    DailyDepartmentalLabourSlipID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyDepartmentalLabourSlipDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                        column: x => x.DailyDepartmentalLabourSlipID,
                        principalTable: "DailyDepartmentalLabourSlips",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DailyLaborDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyLabourID = table.Column<int>(type: "integer", nullable: false),
                    ContractorID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CategoryID = table.Column<int>(type: "integer", nullable: true),
                    Skilled = table.Column<int>(type: "integer", nullable: true),
                    UnSkilled = table.Column<int>(type: "integer", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Mat = table.Column<int>(type: "integer", nullable: true),
                    ContractorName = table.Column<string>(type: "text", nullable: true),
                    ProductivityID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLaborDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DailyLaborDetails_DailyLabor_DailyLabourID",
                        column: x => x.DailyLabourID,
                        principalTable: "DailyLabor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManpowerDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ManpowerID = table.Column<int>(type: "integer", nullable: false),
                    ContractorID = table.Column<int>(type: "integer", nullable: false),
                    ActivityID = table.Column<int>(type: "integer", nullable: false),
                    SkilledCount = table.Column<int>(type: "integer", nullable: false),
                    UnskilledCount = table.Column<int>(type: "integer", nullable: false),
                    OtherCount = table.Column<int>(type: "integer", nullable: false),
                    TotalCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManpowerDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ManpowerDetails_Manpowers_ManpowerID",
                        column: x => x.ManpowerID,
                        principalTable: "Manpowers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DailyDepartmentalLabourSlipID");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLaborDetails_DailyLabourID",
                table: "DailyLaborDetails",
                column: "DailyLabourID");

            migrationBuilder.CreateIndex(
                name: "IX_ManpowerDetails_ManpowerID",
                table: "ManpowerDetails",
                column: "ManpowerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropTable(
                name: "DailyLaborDetails");

            migrationBuilder.DropTable(
                name: "DailyProgressDetails");

            migrationBuilder.DropTable(
                name: "DailyProgresses");

            migrationBuilder.DropTable(
                name: "ManpowerDetails");

            migrationBuilder.DropTable(
                name: "PlanningDetails");

            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.DropTable(
                name: "ProjectActivities");

            migrationBuilder.DropTable(
                name: "RateMasters");

            migrationBuilder.DropTable(
                name: "SiteDailyProgresses");

            migrationBuilder.DropTable(
                name: "DailyDepartmentalLabourSlips");

            migrationBuilder.DropTable(
                name: "DailyLabor");

            migrationBuilder.DropTable(
                name: "Manpowers");
        }
    }
}
