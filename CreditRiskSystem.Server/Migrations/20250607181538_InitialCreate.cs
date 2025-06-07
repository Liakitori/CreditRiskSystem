using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Код1100 = table.Column<double>(type: "double precision", nullable: false),
                    Код1110 = table.Column<double>(type: "double precision", nullable: false),
                    Код1150 = table.Column<double>(type: "double precision", nullable: false),
                    Код1200 = table.Column<double>(type: "double precision", nullable: false),
                    Код1210 = table.Column<double>(type: "double precision", nullable: false),
                    Код1220 = table.Column<double>(type: "double precision", nullable: false),
                    Код1230 = table.Column<double>(type: "double precision", nullable: false),
                    Код1240 = table.Column<double>(type: "double precision", nullable: false),
                    Код1250 = table.Column<double>(type: "double precision", nullable: false),
                    Код1260 = table.Column<double>(type: "double precision", nullable: false),
                    Код1300 = table.Column<double>(type: "double precision", nullable: false),
                    Код1370 = table.Column<double>(type: "double precision", nullable: false),
                    Код1400 = table.Column<double>(type: "double precision", nullable: false),
                    Код1500 = table.Column<double>(type: "double precision", nullable: false),
                    Код1510 = table.Column<double>(type: "double precision", nullable: false),
                    Код1520 = table.Column<double>(type: "double precision", nullable: false),
                    Код1530 = table.Column<double>(type: "double precision", nullable: false),
                    Код1540 = table.Column<double>(type: "double precision", nullable: false),
                    Код1550 = table.Column<double>(type: "double precision", nullable: false),
                    Код1600 = table.Column<double>(type: "double precision", nullable: false),
                    Код1700 = table.Column<double>(type: "double precision", nullable: false),
                    Код2100 = table.Column<double>(type: "double precision", nullable: false),
                    Код2110 = table.Column<double>(type: "double precision", nullable: false),
                    Код2120 = table.Column<double>(type: "double precision", nullable: false),
                    Код2200 = table.Column<double>(type: "double precision", nullable: false),
                    Код2210 = table.Column<double>(type: "double precision", nullable: false),
                    Код2220 = table.Column<double>(type: "double precision", nullable: false),
                    Код2300 = table.Column<double>(type: "double precision", nullable: false),
                    Код2330 = table.Column<double>(type: "double precision", nullable: false),
                    Код2350 = table.Column<double>(type: "double precision", nullable: false),
                    Код2400 = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessmentResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    Р1 = table.Column<double>(type: "double precision", nullable: false),
                    Р2 = table.Column<double>(type: "double precision", nullable: false),
                    Р3 = table.Column<double>(type: "double precision", nullable: false),
                    Р4 = table.Column<double>(type: "double precision", nullable: false),
                    Р5 = table.Column<double>(type: "double precision", nullable: false),
                    Р6 = table.Column<double>(type: "double precision", nullable: false),
                    Р7 = table.Column<double>(type: "double precision", nullable: false),
                    ДА1 = table.Column<double>(type: "double precision", nullable: false),
                    ДА2 = table.Column<double>(type: "double precision", nullable: false),
                    ДА3 = table.Column<double>(type: "double precision", nullable: false),
                    ДА4 = table.Column<double>(type: "double precision", nullable: false),
                    ДА5 = table.Column<double>(type: "double precision", nullable: false),
                    ДА6 = table.Column<double>(type: "double precision", nullable: false),
                    ДА7 = table.Column<double>(type: "double precision", nullable: false),
                    ДА8 = table.Column<double>(type: "double precision", nullable: false),
                    ДА9 = table.Column<double>(type: "double precision", nullable: false),
                    ДА10 = table.Column<double>(type: "double precision", nullable: false),
                    ДА11 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ1 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ2 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ3 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ4 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ5 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ6 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ7 = table.Column<double>(type: "double precision", nullable: false),
                    ФУ8 = table.Column<double>(type: "double precision", nullable: false),
                    П1 = table.Column<double>(type: "double precision", nullable: false),
                    П2 = table.Column<double>(type: "double precision", nullable: false),
                    П3 = table.Column<double>(type: "double precision", nullable: false),
                    П4 = table.Column<double>(type: "double precision", nullable: false),
                    П5 = table.Column<double>(type: "double precision", nullable: false),
                    П6 = table.Column<double>(type: "double precision", nullable: false),
                    П7 = table.Column<double>(type: "double precision", nullable: false),
                    П8 = table.Column<double>(type: "double precision", nullable: false)
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
                name: "IX_FinancialData_UserId",
                table: "FinancialData",
                column: "UserId");

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

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
