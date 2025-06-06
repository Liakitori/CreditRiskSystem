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
                    Код1200 = table.Column<double>(type: "double precision", nullable: false),
                    Код1300 = table.Column<double>(type: "double precision", nullable: false),
                    Код1370 = table.Column<double>(type: "double precision", nullable: false),
                    Код1400 = table.Column<double>(type: "double precision", nullable: false),
                    Код1500 = table.Column<double>(type: "double precision", nullable: false),
                    Код1600 = table.Column<double>(type: "double precision", nullable: false),
                    Код2110 = table.Column<double>(type: "double precision", nullable: false),
                    Код2300 = table.Column<double>(type: "double precision", nullable: false),
                    Код2330 = table.Column<double>(type: "double precision", nullable: false),
                    Код2400 = table.Column<double>(type: "double precision", nullable: false),
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
                    AltmanRiskLevel = table.Column<string>(type: "text", nullable: false),
                    SpringateScore = table.Column<double>(type: "double precision", nullable: false),
                    SpringateRiskLevel = table.Column<string>(type: "text", nullable: false),
                    FulmerScore = table.Column<double>(type: "double precision", nullable: false),
                    FulmerRiskLevel = table.Column<string>(type: "text", nullable: false),
                    OhlsonOScore = table.Column<double>(type: "double precision", nullable: false),
                    OhlsonProbability = table.Column<double>(type: "double precision", nullable: false),
                    ZmijewskiScore = table.Column<double>(type: "double precision", nullable: false),
                    ZmijewskiProbability = table.Column<double>(type: "double precision", nullable: false),
                    OverallRiskAssessment = table.Column<string>(type: "text", nullable: false),
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
