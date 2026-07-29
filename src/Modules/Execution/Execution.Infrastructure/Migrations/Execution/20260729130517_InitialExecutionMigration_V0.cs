using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration_V0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "execution");

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "execution",
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
                schema: "execution",
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
                schema: "execution",
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
                schema: "execution",
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
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_DailyProgresses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Manpowers",
                schema: "execution",
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
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanningID = table.Column<int>(type: "integer", nullable: false),
                    AreaID = table.Column<int>(type: "integer", nullable: false),
                    ActivityID = table.Column<int>(type: "integer", nullable: false),
                    TargetQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UOMID = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Plannings",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    PlanType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectActivities",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    ActivityID = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActivities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RateMasters",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    ActivityID = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    UOMID = table.Column<int>(type: "integer", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SiteDailyProgresses",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SiteDailyProgresses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DailyDepartmentalLabourSlipDetails",
                schema: "execution",
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
                        principalSchema: "execution",
                        principalTable: "DailyDepartmentalLabourSlips",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DailyLaborDetails",
                schema: "execution",
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
                        principalSchema: "execution",
                        principalTable: "DailyLabor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManpowerDetails",
                schema: "execution",
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
                        principalSchema: "execution",
                        principalTable: "Manpowers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DailyDepartmentalLabourSlipID");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLaborDetails_DailyLabourID",
                schema: "execution",
                table: "DailyLaborDetails",
                column: "DailyLabourID");

            migrationBuilder.CreateIndex(
                name: "IX_ManpowerDetails_ManpowerID",
                schema: "execution",
                table: "ManpowerDetails",
                column: "ManpowerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyDepartmentalLabourSlipDetails",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyLaborDetails",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyProgressDetails",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyProgresses",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "ManpowerDetails",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "PlanningDetails",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "Plannings",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "ProjectActivities",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "RateMasters",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "SiteDailyProgresses",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyDepartmentalLabourSlips",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "DailyLabor",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "Manpowers",
                schema: "execution");
        }
    }
}
