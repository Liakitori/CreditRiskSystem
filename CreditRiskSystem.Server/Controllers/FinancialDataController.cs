using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CreditRiskSystem.Common.Models;
using CreditRiskSystem.Server.Data;

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
        public async Task<ActionResult<RiskAssessmentResult>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            // Извлечение данных из файла
            var financialData = ExtractFinancialData(workbook);

            // Расчет кредитного риска
            var result = CalculateRiskAssessment(financialData);

            // Сохранение данных
            financialData.Id = Guid.NewGuid();
            financialData.CreatedAt = DateTime.UtcNow;
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
        public async Task<ActionResult<FinancialData>> GetFinancialData(Guid id)
        {
            var financialData = await _context.FinancialData.FindAsync(id);
            if (financialData == null)
            {
                return NotFound();
            }
            return financialData;
        }

        private FinancialData ExtractFinancialData(XLWorkbook workbook)
        {
            var financialData = new FinancialData();

            // Выводим имена всех листов для отладки
            Console.WriteLine("Available worksheets:");
            foreach (var sheet in workbook.Worksheets)
            {
                Console.WriteLine($" - {sheet.Name}");
            }

            // Бухгалтерский баланс
            var balanceSheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Бухгалтерский баланс", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Лист 'Бухгалтерский баланс' не найден.");
            financialData.Код1200 = GetValueFromCell(balanceSheet, "1200");
            financialData.Код1300 = GetValueFromCell(balanceSheet, "1300");
            financialData.Код1370 = GetValueFromCell(balanceSheet, "1370");
            financialData.Код1400 = GetValueFromCell(balanceSheet, "1400");
            financialData.Код1500 = GetValueFromCell(balanceSheet, "1500");
            financialData.Код1600 = GetValueFromCell(balanceSheet, "1600");

            // Отчет о финансовых результатах
            var incomeStatement = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Отчет о финансовых результатах", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Лист 'Отчет о финансовых результатах' не найден.");
            financialData.Код2110 = GetValueFromCell(incomeStatement, "2110");
            financialData.Код2300 = GetValueFromCell(incomeStatement, "2300");
            financialData.Код2330 = GetValueFromCell(incomeStatement, "2330");
            financialData.Код2400 = GetValueFromCell(incomeStatement, "2400");

            return financialData;
        }

        private double GetValueFromCell(IXLWorksheet worksheet, string code)
        {
            // Определяем диапазон столбцов для кодов в зависимости от листа
            int codeStartColumn, codeEndColumn, valueColumn;
            if (worksheet.Name.Equals("Бухгалтерский баланс", StringComparison.OrdinalIgnoreCase))
            {
                codeStartColumn = 14; // Столбец N
                codeEndColumn = 17;   // Столбец Q
                valueColumn = 18;     // Столбец R (первая ячейка диапазона R-X)
            }
            else if (worksheet.Name.Equals("Отчет о финансовых результатах", StringComparison.OrdinalIgnoreCase))
            {
                codeStartColumn = 17; // Столбец Q
                codeEndColumn = 22;   // Столбец V
                valueColumn = 23;     // Столбец W (первая ячейка диапазона W-AC)
            }
            else
            {
                Console.WriteLine($"Unknown worksheet: {worksheet.Name}");
                return 0;
            }

            // Ищем ячейку с кодом строки в диапазоне столбцов
            var cell = worksheet.CellsUsed()
                .FirstOrDefault(c =>
                {
                    var colNum = c.WorksheetColumn().ColumnNumber();
                    return colNum >= codeStartColumn && colNum <= codeEndColumn &&
                           c.Value.ToString().Trim().Equals(code, StringComparison.OrdinalIgnoreCase);
                });

            if (cell != null)
            {
                // Извлекаем значение из столбца valueColumn в той же строке
                var valueCell = cell.WorksheetRow().Cell(valueColumn);
                var rawValue = valueCell.Value.ToString().Trim();

                Console.WriteLine($"Found cell for code {code} at {cell.Address}, raw value in {valueCell.Address}: {rawValue}");

                // Обрабатываем возможные форматы чисел
                string cleanedValue = rawValue
                    .Replace(",", ".") // Заменяем запятые на точки
                    .Replace(" ", ""); // Удаляем пробелы, например 23 208 -> 23208

                // Для Код2330 убираем скобки, но не добавляем минус
                if (code.Equals("2330", StringComparison.OrdinalIgnoreCase))
                {
                    cleanedValue = cleanedValue.Replace("(", "").Replace(")", "");
                }
                else
                {
                    // Для остальных кодов скобки означают отрицательное значение
                    cleanedValue = cleanedValue.Replace("(", "-").Replace(")", "");
                }

                if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double value))
                {
                    Console.WriteLine($"Parsed value for code {code}: {value}");
                    return value;
                }
                else
                {
                    Console.WriteLine($"Failed to parse value for code {code}: {cleanedValue}");
                    return 0;
                }
            }

            Console.WriteLine($"Cell with code {code} not found in worksheet {worksheet.Name}");
            return 0;
        }

        private RiskAssessmentResult CalculateRiskAssessment(FinancialData data)
        {
            var result = new RiskAssessmentResult();

            // Вспомогательная функция для безопасного деления
            double SafeDivide(double numerator, double denominator)
            {
                return denominator != 0 ? numerator / denominator : 0;
            }

            // Вспомогательная функция для безопасного логарифма
            double SafeLog(double value)
            {
                return value > 0 ? Math.Log(value) : 0;
            }

            // Altman Z-score
            double x1 = SafeDivide(data.Код1200 - data.Код1500, data.Код1600);
            double x2 = SafeDivide(data.Код1370, data.Код1600);
            double x3 = SafeDivide(data.Код2300, data.Код1600);
            double x4 = SafeDivide(data.Код1300, data.Код1400 + data.Код1500);
            double x5 = SafeDivide(data.Код2110, data.Код1600);
            result.AltmanZScore = double.IsNaN(0.717 * x1 + 0.847 * x2 + 3.107 * x3 + 0.420 * x4 + 0.998 * x5)
                ? 0
                : 0.717 * x1 + 0.847 * x2 + 3.107 * x3 + 0.420 * x4 + 0.998 * x5;
            result.AltmanRiskLevel = result.AltmanZScore > 2.9 ? "Низкий" : (result.AltmanZScore > 1.23 ? "Средний" : "Высокий");

            // Springate
            double a = SafeDivide(data.Код1200 - data.Код1500, data.Код1600);
            double b = SafeDivide(data.Код2300, data.Код1600);
            double c = SafeDivide(data.Код2300, data.Код1500);
            double d = SafeDivide(data.Код2110, data.Код1600);
            result.SpringateScore = double.IsNaN(1.03 * a + 3.07 * b + 0.66 * c + 0.4 * d)
                ? 0
                : 1.03 * a + 3.07 * b + 0.66 * c + 0.4 * d;
            result.SpringateRiskLevel = result.SpringateScore > 0.862 ? "Низкий" : "Высокий";

            // Fulmer
            double v1 = SafeDivide(data.Код1370, data.Код1600);
            double v2 = SafeDivide(data.Код2110, data.Код1600);
            double v3 = SafeDivide(data.Код2300, data.Код1300);
            double v4 = SafeDivide(data.Код2400, data.Код1400 + data.Код1500);
            double v5 = SafeDivide(data.Код1400 + data.Код1500, data.Код1600);
            double v6 = SafeDivide(data.Код1500, data.Код1600);
            double v7 = SafeLog(data.Код1600);
            double v8 = SafeDivide(data.Код1200 - data.Код1500, data.Код1400 + data.Код1500);
            double v9 = data.Код2330 > 0 ? SafeLog(data.Код2300 / data.Код2330) : 0;
            result.FulmerScore = double.IsNaN(5.528 * v1 + 0.212 * v2 + 0.073 * v3 + 1.270 * v4 - 0.120 * v5 + 2.335 * v6 + 0.575 * v7 + 1.083 * v8 + 0.894 * v9 - 6.075)
                ? 0
                : 5.528 * v1 + 0.212 * v2 + 0.073 * v3 + 1.270 * v4 - 0.120 * v5 + 2.335 * v6 + 0.575 * v7 + 1.083 * v8 + 0.894 * v9 - 6.075;
            result.FulmerRiskLevel = result.FulmerScore > 0 ? "Низкий" : "Высокий";

            // Ohlson O-score
            double o = -1.32 - 0.407 * SafeLog(data.Код1600)
                + 6.03 * SafeDivide(data.Код1400 + data.Код1500, data.Код1600)
                - 1.43 * SafeDivide(data.Код1200 - data.Код1500, data.Код1600)
                + 0.076 * SafeDivide(data.Код1500, data.Код1200)
                - 1.72 * ((data.Код1400 + data.Код1500) > data.Код1600 ? 1 : 0)
                - 2.37 * SafeDivide(data.Код2400, data.Код1600)
                - 1.83 * SafeDivide(data.Код2300, data.Код1400 + data.Код1500)
                + 0.285 * (data.Код2400 < 0 ? 1 : 0)
                - 0.521 * 0; // CHIN не учитываем без прошлогодних данных
            result.OhlsonOScore = double.IsNaN(o) ? 0 : o;
            result.OhlsonProbability = 1 / (1 + Math.Exp(-result.OhlsonOScore));

            // Zmijewski
            double x = -4.3 - 4.5 * SafeDivide(data.Код2400, data.Код1600)
                + 5.7 * SafeDivide(data.Код1400 + data.Код1500, data.Код1600)
                - 0.004 * SafeDivide(data.Код1200, data.Код1500);
            result.ZmijewskiScore = double.IsNaN(x) ? 0 : x;
            result.ZmijewskiProbability = 1 / (1 + Math.Exp(-result.ZmijewskiScore));

            // Общая оценка кредитного риска
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