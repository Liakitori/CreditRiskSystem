using ClosedXML.Excel;
using CreditRiskSystem.Common.Models;
using CreditRiskSystem.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Security;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CreditRiskSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinancialDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ActionResult<RiskAssessmentResult>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не загружен.");

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var financialData = ExtractFinancialData(workbook);
            var result = CalculateRiskAssessment(financialData);

            financialData.Id = Guid.NewGuid();
            financialData.CreatedAt = DateTime.UtcNow;
            financialData.UserId = userId;
            result.Id = Guid.NewGuid();
            result.FinancialDataId = financialData.Id;
            result.CalculatedAt = DateTime.UtcNow;
            result.FinancialData = financialData;

            _context.FinancialData.Add(financialData);
            _context.RiskAssessmentResults.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFinancialData), new { id = financialData.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FinancialData>> GetFinancialData(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var financialData = await _context.FinancialData
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (financialData == null) return NotFound();
            return financialData;
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<ActionResult<List<RiskAssessmentResult>>> GetHistory()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var results = await _context.RiskAssessmentResults
                .Include(r => r.FinancialData)
                .Where(r => r.FinancialData.UserId == userId)
                .OrderByDescending(r => r.CalculatedAt)
                .ToListAsync();
            return results;
        }

        [HttpGet("download/pdf/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadPdf(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _context.RiskAssessmentResults
                .Include(r => r.FinancialData)
                .FirstOrDefaultAsync(r => r.Id == id && r.FinancialData.UserId == userId);
            if (result == null) return NotFound("Результат не найден или доступ запрещён.");

            var sb = new StringBuilder();
            sb.AppendLine($"Отчет № {result.Id}");
            sb.AppendLine($"Дата создания: {result.CalculatedAt:dd.MM.yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine("=== Модели кредитного риска ===");
            sb.AppendLine($"Altman Z-score: {result.AltmanZScore:F2} ({result.AltmanRiskLevel})");
            sb.AppendLine($"Springate: {result.SpringateScore:F2} ({result.SpringateRiskLevel})");
            sb.AppendLine($"Fulmer: {result.FulmerScore:F2} ({result.FulmerRiskLevel})");
            sb.AppendLine($"Ohlson O-score: {result.OhlsonOScore:F2} (Вероятность: {result.OhlsonProbability:F2})");
            sb.AppendLine($"Zmijewski: {result.ZmijewskiScore:F2} (Вероятность: {result.ZmijewskiProbability:F2})");
            sb.AppendLine($"Общая оценка кредитного риска: {result.OverallRiskAssessment}");
            sb.AppendLine();
            sb.AppendLine("=== Рентабельность ===");
            sb.AppendLine($"Рентабельность объема продаж (Р1): {result.Р1:F2}%");
            sb.AppendLine($"Бухгалтерская рентабельность (Р2): {result.Р2:F2}%");
            sb.AppendLine($"Чистая рентабельность (Р3): {result.Р3:F2}%");
            sb.AppendLine($"Экономическая рентабельность (Р4): {result.Р4:F2}%");
            sb.AppendLine($"Рентабельность собственного капитала (Р5): {result.Р5:F2}%");
            sb.AppendLine($"Валовая рентабельность (Р6): {result.Р6:F2}%");
            sb.AppendLine($"Рентабельность реализованной продукции (Р7): {result.Р7:F2}%");
            sb.AppendLine();
            sb.AppendLine("=== Деловая активность ===");
            sb.AppendLine($"Общая оборачиваемость капитала (ДА1): {result.ДА1:F2} оборотов");
            sb.AppendLine($"Оборачиваемость оборотных средств (ДА2): {result.ДА2:F2} оборотов");
            sb.AppendLine($"Отдача нематериальных активов (ДА3): {result.ДА3:F2} оборотов");
            sb.AppendLine($"Фондоотдача (ДА4): {result.ДА4:F2} оборотов");
            sb.AppendLine($"Отдача собственного капитала (ДА5): {result.ДА5:F2} оборотов");
            sb.AppendLine($"Оборачиваемость средств в расчетах (ДА6): {result.ДА6:F2} оборотов");
            sb.AppendLine($"Оборачиваемость кредиторской задолженности (ДА7): {result.ДА7:F2} оборотов");
            sb.AppendLine($"Оборачиваемость материальных средств (ДА8): {result.ДА8:F2} дней");
            sb.AppendLine($"Оборачиваемость денежных средств (ДА9): {result.ДА9:F2} дней");
            sb.AppendLine($"Срок погашения дебиторской задолженности (ДА10): {result.ДА10:F2} дней");
            sb.AppendLine($"Срок погашения кредиторской задолженности (ДА11): {result.ДА11:F2} дней");
            sb.AppendLine();
            sb.AppendLine("=== Финансовая устойчивость ===");
            sb.AppendLine($"Коэффициент капитализации (ФУ1): {result.ФУ1:F2}");
            sb.AppendLine($"Собственный капитал в обороте (ФУ2): {result.ФУ2:F2} тыс. руб.");
            sb.AppendLine($"Обеспеченность запасов собственными источниками (ФУ3): {result.ФУ3:F2}");
            sb.AppendLine($"Коэффициент автономии (ФУ4): {result.ФУ4:F2}");
            sb.AppendLine($"Коэффициент финансирования (ФУ5): {result.ФУ5:F2}");
            sb.AppendLine($"Коэффициент финансовой устойчивости (ФУ6): {result.ФУ6:F2}");
            sb.AppendLine($"Коэффициент маневренности (ФУ7): {result.ФУ7:F2}");
            sb.AppendLine($"Коэффициент мобилизации (ФУ8): {result.ФУ8:F2}");
            sb.AppendLine();
            sb.AppendLine("=== Платёжеспособность ===");
            sb.AppendLine($"Общий показатель платежеспособности (П1): {result.П1:F2}");
            sb.AppendLine($"Коэффициент абсолютной ликвидности (П2): {result.П2:F2}");
            sb.AppendLine($"Коэффициент быстрой ликвидности (П3): {result.П3:F2}");
            sb.AppendLine($"Коэффициент текущей ликвидности (П4): {result.П4:F2}");
            sb.AppendLine($"Коэффициент маневренности функционирующего капитала (П5): {result.П5:F2}");
            sb.AppendLine($"Доля оборотных средств в активах (П6): {result.П6:F2}");
            sb.AppendLine($"Коэффициент обеспеченности собственными средствами (П7): {result.П7:F2}");
            sb.AppendLine($"Коэффициент обеспеченности обязательств активами (П8): {result.П8:F2}");

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Результат оценки кредитного риска";
            document.SecuritySettings.PermitPrint = true;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont headerFont = new XFont("Verdana", 16, XFontStyle.Bold);
            XFont sectionFont = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont textFont = new XFont("Verdana", 12, XFontStyle.Regular);
            double margin = 40;
            double yPoint = margin;
            double spacingHeader = 30;
            double spacingSection = 20;
            double spacingText = 15;

            var lines = sb.ToString().Split('\n');
            foreach (var line in lines)
            {
                XFont font = line.StartsWith("Отчет №") ? headerFont :
                             line.StartsWith("===") ? sectionFont : textFont;
                double spacing = line.StartsWith("Отчет №") ? spacingHeader :
                                 line.StartsWith("===") ? spacingSection : spacingText;

                if (yPoint + spacing > page.Height - margin)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = margin;
                }

                gfx.DrawString(line.TrimEnd(), font, XBrushes.Black,
                    new XRect(margin, yPoint, page.Width - 2 * margin, spacing), XStringFormats.TopLeft);
                yPoint += spacing;
            }

            string watermark = "Сгенерировано системой CreditRiskSystem";
            XFont watermarkFont = new XFont("Verdana", 15, XFontStyle.Italic);
            XSize watermarkSize = gfx.MeasureString(watermark, watermarkFont);
            double watermarkX = page.Width - margin - 20;
            double watermarkY = margin;
            gfx.Save();
            gfx.TranslateTransform(watermarkX, watermarkY);
            gfx.RotateTransform(90);
            gfx.DrawString(watermark, watermarkFont, new XSolidBrush(XColor.FromArgb(128, 0, 0, 0)),
                new XRect(0, 0, watermarkSize.Width, watermarkSize.Height), XStringFormats.Center);
            gfx.Restore();

            using MemoryStream ms = new MemoryStream();
            document.Save(ms, false);
            return File(ms.ToArray(), "application/pdf", $"result_{id}.pdf");
        }

        [HttpGet("download/json/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadJson(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _context.RiskAssessmentResults
                .Include(r => r.FinancialData)
                .FirstOrDefaultAsync(r => r.Id == id && r.FinancialData.UserId == userId);
            if (result == null) return NotFound("Результат не найден или доступ запрещён.");

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            return File(jsonBytes, "application/json", $"result_{id}.json");
        }

        private FinancialData ExtractFinancialData(XLWorkbook workbook)
        {
            var financialData = new FinancialData();

            var balanceSheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Бухгалтерский баланс", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Лист 'Бухгалтерский баланс' не найден.");
            financialData.Код1100 = GetValueFromCell(balanceSheet, "1100");
            financialData.Код1110 = GetValueFromCell(balanceSheet, "1110");
            financialData.Код1150 = GetValueFromCell(balanceSheet, "1150");
            financialData.Код1200 = GetValueFromCell(balanceSheet, "1200");
            financialData.Код1210 = GetValueFromCell(balanceSheet, "1210");
            financialData.Код1220 = GetValueFromCell(balanceSheet, "1220");
            financialData.Код1230 = GetValueFromCell(balanceSheet, "1230");
            financialData.Код1240 = GetValueFromCell(balanceSheet, "1240");
            financialData.Код1250 = GetValueFromCell(balanceSheet, "1250");
            financialData.Код1260 = GetValueFromCell(balanceSheet, "1260");
            financialData.Код1300 = GetValueFromCell(balanceSheet, "1300");
            financialData.Код1370 = GetValueFromCell(balanceSheet, "1370");
            financialData.Код1400 = GetValueFromCell(balanceSheet, "1400");
            financialData.Код1500 = GetValueFromCell(balanceSheet, "1500");
            financialData.Код1510 = GetValueFromCell(balanceSheet, "1510");
            financialData.Код1520 = GetValueFromCell(balanceSheet, "1520");
            financialData.Код1530 = GetValueFromCell(balanceSheet, "1530");
            financialData.Код1540 = GetValueFromCell(balanceSheet, "1540");
            financialData.Код1550 = GetValueFromCell(balanceSheet, "1550");
            financialData.Код1600 = GetValueFromCell(balanceSheet, "1600");
            financialData.Код1700 = GetValueFromCell(balanceSheet, "1700");

            var incomeStatement = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Отчет о финансовых результатах", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Лист 'Отчет о финансовых результатах' не найден.");
            financialData.Код2100 = GetValueFromCell(incomeStatement, "2100");
            financialData.Код2110 = GetValueFromCell(incomeStatement, "2110");
            financialData.Код2120 = GetValueFromCell(incomeStatement, "2120");
            financialData.Код2200 = GetValueFromCell(incomeStatement, "2200");
            financialData.Код2210 = GetValueFromCell(incomeStatement, "2210");
            financialData.Код2220 = GetValueFromCell(incomeStatement, "2220");
            financialData.Код2300 = GetValueFromCell(incomeStatement, "2300");
            financialData.Код2330 = GetValueFromCell(incomeStatement, "2330");
            financialData.Код2350 = GetValueFromCell(incomeStatement, "2350");
            financialData.Код2400 = GetValueFromCell(incomeStatement, "2400");

            return financialData;
        }

        private double GetValueFromCell(IXLWorksheet worksheet, string code)
        {
            int codeStartColumn, codeEndColumn, valueColumn;
            if (worksheet.Name.Equals("Бухгалтерский баланс", StringComparison.OrdinalIgnoreCase))
            {
                codeStartColumn = 14;
                codeEndColumn = 17;
                valueColumn = 18;
            }
            else if (worksheet.Name.Equals("Отчет о финансовых результатах", StringComparison.OrdinalIgnoreCase))
            {
                codeStartColumn = 17;
                codeEndColumn = 22;
                valueColumn = 23;
            }
            else
            {
                return 0;
            }

            var cell = worksheet.CellsUsed()
                .FirstOrDefault(c =>
                {
                    var colNum = c.WorksheetColumn().ColumnNumber();
                    return colNum >= codeStartColumn && colNum <= codeEndColumn &&
                           c.Value.ToString().Trim().Equals(code, StringComparison.OrdinalIgnoreCase);
                });

            if (cell != null)
            {
                var valueCell = cell.WorksheetRow().Cell(valueColumn);
                var rawValue = valueCell.Value.ToString().Trim();
                string cleanedValue = rawValue.Replace(",", ".").Replace(" ", "");
                if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double value))
                {
                    return value;
                }
            }

            return 0;
        }

        private RiskAssessmentResult CalculateRiskAssessment(FinancialData data)
        {
            var result = new RiskAssessmentResult();

            double SafeDivide(double numerator, double denominator) => denominator != 0 ? numerator / denominator : 0;
            double SafeLog(double value) => value > 0 ? Math.Log(value) : 0;

            double x1 = SafeDivide(data.Код1200 - data.Код1500, data.Код1600);
            double x2 = SafeDivide(data.Код1370, data.Код1600);
            double x3 = SafeDivide(data.Код2300, data.Код1600);
            double x4 = SafeDivide(data.Код1300, data.Код1400 + data.Код1500);
            double x5 = SafeDivide(data.Код2110, data.Код1600);
            result.AltmanZScore = 0.717 * x1 + 0.847 * x2 + 3.107 * x3 + 0.420 * x4 + 0.998 * x5;
            result.AltmanRiskLevel = result.AltmanZScore > 2.9 ? "Низкий" : (result.AltmanZScore > 1.23 ? "Средний" : "Высокий");

            double a = SafeDivide(data.Код1200 - data.Код1500, data.Код1600);
            double b = SafeDivide(data.Код2300, data.Код1600);
            double c = SafeDivide(data.Код2300, data.Код1500);
            double d = SafeDivide(data.Код2110, data.Код1600);
            result.SpringateScore = 1.03 * a + 3.07 * b + 0.66 * c + 0.4 * d;
            result.SpringateRiskLevel = result.SpringateScore > 0.862 ? "Низкий" : "Высокий";

            double v1 = SafeDivide(data.Код1370, data.Код1600);
            double v2 = SafeDivide(data.Код2110, data.Код1600);
            double v3 = SafeDivide(data.Код2300, data.Код1300);
            double v4 = SafeDivide(data.Код2400, data.Код1400 + data.Код1500);
            double v5 = SafeDivide(data.Код1400 + data.Код1500, data.Код1600);
            double v6 = SafeDivide(data.Код1500, data.Код1600);
            double v7 = SafeLog(data.Код1600);
            double v8 = SafeDivide(data.Код1200 - data.Код1500, data.Код1400 + data.Код1500);
            double v9 = data.Код2330 > 0 ? SafeLog(data.Код2300 / data.Код2330) : 0;
            result.FulmerScore = 5.528 * v1 + 0.212 * v2 + 0.073 * v3 + 1.270 * v4 - 0.120 * v5 + 2.335 * v6 + 0.575 * v7 + 1.083 * v8 + 0.894 * v9 - 6.075;
            result.FulmerRiskLevel = result.FulmerScore > 0 ? "Низкий" : "Высокий";

            double o = -1.32 - 0.407 * SafeLog(data.Код1600)
                + 6.03 * SafeDivide(data.Код1400 + data.Код1500, data.Код1600)
                - 1.43 * SafeDivide(data.Код1200 - data.Код1500, data.Код1600)
                + 0.076 * SafeDivide(data.Код1500, data.Код1200)
                - 1.72 * ((data.Код1400 + data.Код1500) > data.Код1600 ? 1 : 0)
                - 2.37 * SafeDivide(data.Код2400, data.Код1600)
                - 1.83 * SafeDivide(data.Код2300, data.Код1400 + data.Код1500)
                + 0.285 * (data.Код2400 < 0 ? 1 : 0)
                - 0.521 * 0;
            result.OhlsonOScore = o;
            result.OhlsonProbability = 1 / (1 + Math.Exp(-result.OhlsonOScore));

            double x = -4.3 - 4.5 * SafeDivide(data.Код2400, data.Код1600)
                + 5.7 * SafeDivide(data.Код1400 + data.Код1500, data.Код1600)
                - 0.004 * SafeDivide(data.Код1200, data.Код1500);
            result.ZmijewskiScore = x;
            result.ZmijewskiProbability = 1 / (1 + Math.Exp(-result.ZmijewskiScore));

            result.Р1 = SafeDivide(data.Код2200 * 100, data.Код2110);
            result.Р2 = SafeDivide(data.Код2300 * 100, data.Код2110);
            result.Р3 = SafeDivide(data.Код2400 * 100, data.Код2110);
            result.Р4 = SafeDivide(data.Код2400 * 100, data.Код1600);
            result.Р5 = SafeDivide(data.Код2400 * 100, data.Код1300);
            result.Р6 = SafeDivide(data.Код2100 * 100, data.Код2110);
            result.Р7 = SafeDivide(data.Код2200 * 100, data.Код2120 + data.Код2210 + data.Код2220 + data.Код2350);

            result.ДА1 = SafeDivide(data.Код2110, data.Код1600);
            result.ДА2 = SafeDivide(data.Код2110, data.Код1200);
            result.ДА3 = SafeDivide(data.Код2110, data.Код1110);
            result.ДА4 = SafeDivide(data.Код2110, data.Код1150);
            result.ДА5 = SafeDivide(data.Код2110, data.Код1370);
            result.ДА6 = SafeDivide(data.Код2110, data.Код1230);
            result.ДА7 = SafeDivide(data.Код2110, data.Код1510);
            result.ДА8 = SafeDivide(data.Код1210 * 365, data.Код2310);
            result.ДА9 = SafeDivide(data.Код1250 * 365, data.Код2310);
            result.ДА10 = SafeDivide(data.Код1230 * 365, data.Код2310);
            result.ДА11 = SafeDivide(data.Код1520 * 365, data.Код2310);

            result.ФУ1 = SafeDivide(data.Код1400 + data.Код1500, data.Код1300);
            result.ФУ2 = data.Код1300 - data.Код1100;
            result.ФУ3 = SafeDivide(data.Код1300 - data.Код1100, data.Код1210 + data.Код1220);
            result.ФУ4 = SafeDivide(data.Код1300, data.Код1700);
            result.ФУ5 = SafeDivide(data.Код1300, data.Код1400 + data.Код1500);
            result.ФУ6 = SafeDivide(data.Код1300 + data.Код1400, data.Код1700);
            result.ФУ7 = SafeDivide(data.Код1300 - data.Код1100, data.Код1300);
            result.ФУ8 = SafeDivide(data.Код1100, data.Код1200);

            result.П1 = SafeDivide(
                (data.Код1250 + data.Код1240) + 0.5 * data.Код1230 + 0.3 * (data.Код1210 + data.Код1220 + data.Код1230 + data.Код1240 + data.Код1250 + data.Код1260),
                data.Код1520 + 0.5 * (data.Код1510 + data.Код1550) + 0.3 * (data.Код1540 + data.Код1530 + data.Код1400)
            );
            result.П2 = SafeDivide(data.Код1250 + data.Код1240, data.Код1500);
            result.П3 = SafeDivide(data.Код1250 + data.Код1240 + data.Код1230, data.Код1500);
            result.П4 = SafeDivide(data.Код1200, data.Код1500);
            result.П5 = SafeDivide(data.Код1210 + data.Код1220 + data.Код1230, data.Код1200 - data.Код1500);
            result.П6 = SafeDivide(data.Код1200, data.Код1700);
            result.П7 = SafeDivide(data.Код1300 - data.Код1100, data.Код1200);
            result.П8 = SafeDivide(data.Код1200 + data.Код1100, data.Код1500 + data.Код1400);

            int highRiskCount = 0;
            if (result.AltmanRiskLevel == "Высокий") highRiskCount++;
            if (result.SpringateRiskLevel == "Высокий") highRiskCount++;
            if (result.FulmerRiskLevel == "Высокий") highRiskCount++;
            if (result.OhlsonProbability > 0.5) highRiskCount++;
            if (result.ZmijewskiProbability > 0.5) highRiskCount++;
            result.OverallRiskAssessment = highRiskCount >= 3 ? "Высокий" : (highRiskCount >= 1 ? "Средний" : "Низкий");

            return result;
        }
    }
}