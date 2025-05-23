using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkingCapital = table.Column<double>(type: "double precision", nullable: false),
                    TotalAssets = table.Column<double>(type: "double precision", nullable: false),
                    RetainedEarnings = table.Column<double>(type: "double precision", nullable: false),
                    EBIT = table.Column<double>(type: "double precision", nullable: false),
                    MarketValueOfEquity = table.Column<double>(type: "double precision", nullable: false),
                    TotalLiabilities = table.Column<double>(type: "double precision", nullable: false),
                    Revenue = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessmentResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    AltmanZScore = table.Column<double>(type: "double precision", nullable: false),
                    RiskLevel = table.Column<string>(type: "text", nullable: false),
                    Recommendations = table.Column<string>(type: "text", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessmentResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskAssessmentResults_FinancialData_FinancialDataId",
                        column: x => x.FinancialDataId,
                        principalTable: "FinancialData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessmentResults_FinancialDataId",
                table: "RiskAssessmentResults",
                column: "FinancialDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiskAssessmentResults");

            migrationBuilder.DropTable(
                name: "FinancialData");
        }
    }
}
