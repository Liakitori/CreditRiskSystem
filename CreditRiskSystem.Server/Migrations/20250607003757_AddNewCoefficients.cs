using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewCoefficients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ДА1",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА10",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА11",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА2",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА3",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА4",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА5",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА6",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА7",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА8",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ДА9",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П1",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П2",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П3",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П4",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П5",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П6",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П7",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "П8",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р1",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р2",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р3",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р4",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р5",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р6",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Р7",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ1",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ2",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ3",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ4",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ5",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ6",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ7",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ФУ8",
                table: "RiskAssessmentResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1100",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1110",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1150",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1210",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1220",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1230",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1240",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1250",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1260",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1510",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1520",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1530",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1540",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1550",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код1700",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2100",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2120",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2200",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2210",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2220",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Код2350",
                table: "FinancialData",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ДА1",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА10",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА11",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА2",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА3",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА4",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА5",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА6",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА7",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА8",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ДА9",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П1",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П2",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П3",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П4",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П5",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П6",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П7",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "П8",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р1",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р2",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р3",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р4",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р5",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р6",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Р7",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ1",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ2",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ3",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ4",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ5",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ6",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ7",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ФУ8",
                table: "RiskAssessmentResults");

            migrationBuilder.DropColumn(
                name: "Код1100",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1110",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1150",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1210",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1220",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1230",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1240",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1250",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1260",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1510",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1520",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1530",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1540",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1550",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код1700",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2100",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2120",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2200",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2210",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2220",
                table: "FinancialData");

            migrationBuilder.DropColumn(
                name: "Код2350",
                table: "FinancialData");
        }
    }
}
