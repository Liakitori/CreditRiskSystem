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

            // Отчет о финансовых результатах
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

            //ВОЗМОЖНО КОГДА-ТО ПОНАДОБИТСЯ ТАКОЙ ВАРИАНТ
            /*// Ищем ячейку с кодом строки в диапазоне столбцов, начиная с 9-й строки
            var cell = worksheet.RowsUsed(r => r.RowNumber() >= 9)
                .SelectMany(r => r.CellsUsed())
                .FirstOrDefault(c =>
                {
                    var colNum = c.WorksheetColumn().ColumnNumber();
                    return colNum >= codeStartColumn && colNum <= codeEndColumn &&
                           c.Value.ToString().Trim().Equals(code, StringComparison.OrdinalIgnoreCase);
                });*/


            if (cell != null)
            {
                // Извлекаем значение из столбца valueColumn в той же строке
                var valueCell = cell.WorksheetRow().Cell(valueColumn);
                var rawValue = valueCell.Value.ToString().Trim();

                Console.WriteLine($"Found cell for code {code} at {cell.Address}, raw value in {valueCell.Address}: {rawValue}");

                // Обрабатываем возможные форматы чисел
                string cleanedValue = rawValue
                    .Replace(",", ".") // Заменяем запятые на точки
                    .Replace(" ", "") // Удаляем пробелы, например 23 208 -> 23208
                    .Replace(",", ".") // Заменяем запятые на точки
                    .Replace(" ", ""); // Удаляем пробелы


                //РАСКОММЕНТИТЬ ТОЛЬКО ЕСЛИ НАДО СДЕЛАТЬ КАКИЕ-ТО КОДЫ СО ЗНАКОМ МИНУС, НО ЛУЧШЕ МЕНЯТЬ ЗНАК В САМИХ РАСЧЕТАХ
                /*// Для Код2330 убираем скобки, но не добавляем минус
                if (code.Equals("2330", StringComparison.OrdinalIgnoreCase))
                {
                    cleanedValue = cleanedValue.Replace("(", "").Replace(")", "");
                }
                else
                {
                    // Для остальных кодов скобки означают отрицательное значение
                    cleanedValue = cleanedValue.Replace("(", "-").Replace(")", "");
                }
                // Для Код2330, Код2120, Код2210, Код2220, Код2350 убираем скобки, но не добавляем минус
                if (new[] { "2330", "2120", "2210", "2220", "2350" }.Contains(code, StringComparer.OrdinalIgnoreCase))
                {
                    cleanedValue = cleanedValue.Replace("(", "").Replace(")", "");
                }
                else
                {
                    // Для остальных кодов скобки означают отрицательное значение
                    cleanedValue = cleanedValue.Replace("(", "-").Replace(")", "");
                }*/

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

            // Рентабельность
            result.Р1 = SafeDivide(data.Код2200 * 100, data.Код2110); // Рентабельность объема продаж
            result.Р2 = SafeDivide(data.Код2300 * 100, data.Код2110); // Бухгалтерская рентабельность
            result.Р3 = SafeDivide(data.Код2400 * 100, data.Код2110); // Чистая рентабельность
            result.Р4 = SafeDivide(data.Код2400 * 100, data.Код1600); // Экономическая рентабельность
            result.Р5 = SafeDivide(data.Код2400 * 100, data.Код1300); // Рентабельность собственного капитала
            result.Р6 = SafeDivide(data.Код2100 * 100, data.Код2110); // Валовая рентабельность
            result.Р7 = SafeDivide(data.Код2200 * 100, data.Код2120 + data.Код2210 + data.Код2220 + data.Код2350); // Рентабельность реализованной продукции

            // Деловая активность
            result.ДА1 = SafeDivide(data.Код2110, data.Код1600); // Коэффициент общей оборачиваемости капитала
            result.ДА2 = SafeDivide(data.Код2110, data.Код1200); // Коэффициент оборачиваемости оборотных средств
            result.ДА3 = SafeDivide(data.Код2110, data.Код1110); // Коэффициент отдачи нематериальных активов
            result.ДА4 = SafeDivide(data.Код2110, data.Код1150); // Фондоотдача
            result.ДА5 = SafeDivide(data.Код2110, data.Код1370); // Коэффициент отдачи собственного капитала
            result.ДА6 = SafeDivide(data.Код2110, data.Код1230); // Коэффициент оборачиваемости средств в расчетах
            result.ДА7 = SafeDivide(data.Код2110, data.Код1510); // Коэффициент оборачиваемости кредиторской задолженности
            result.ДА8 = SafeDivide(data.Код1210 * 365, data.Код2110); // Оборачиваемость материальных средств
            result.ДА9 = SafeDivide(data.Код1250 * 365, data.Код2110); // Оборачиваемость денежных средств
            result.ДА10 = SafeDivide(data.Код1230 * 365, data.Код2110); // Срок погашения дебиторской задолженности
            result.ДА11 = SafeDivide(data.Код1520 * 365, data.Код2110); // Срок погашения кредиторской задолженности

            // Финансовая устойчивость
            result.ФУ1 = SafeDivide(data.Код1400 + data.Код1500, data.Код1300); // Коэффициент капитализации
            result.ФУ2 = data.Код1300 - data.Код1100; // Собственный капитал в обороте
            result.ФУ3 = SafeDivide(data.Код1300 - data.Код1100, data.Код1210 + data.Код1220); // Коэффициент обеспеченности запасов
            result.ФУ4 = SafeDivide(data.Код1300, data.Код1700); // Коэффициент автономии
            result.ФУ5 = SafeDivide(data.Код1300, data.Код1400 + data.Код1500); // Коэффициент финансирования
            result.ФУ6 = SafeDivide(data.Код1300 + data.Код1400, data.Код1700); // Коэффициент финансовой устойчивости
            result.ФУ7 = SafeDivide(data.Код1300 - data.Код1100, data.Код1300); // Коэффициент маневренности
            result.ФУ8 = SafeDivide(data.Код1100, data.Код1200); // Коэффициент мобилизации

            // Платёжеспособность
            result.П1 = SafeDivide(
                (data.Код1250 + data.Код1240) + 0.5 * data.Код1230 + 0.3 * (data.Код1210 + data.Код1220 + data.Код1230 + data.Код1240 + data.Код1250 + data.Код1260),
                data.Код1520 + 0.5 * (data.Код1510 + data.Код1550) + 0.3 * (data.Код1540 + data.Код1530 + data.Код1400)
            ); // Общий показатель платежеспособности
            result.П2 = SafeDivide(data.Код1250 + data.Код1240, data.Код1500); // Коэффициент абсолютной ликвидности
            result.П3 = SafeDivide(data.Код1250 + data.Код1240 + data.Код1230, data.Код1500); // Коэффициент быстрой ликвидности
            result.П4 = SafeDivide(data.Код1200, data.Код1500); // Коэффициент текущей ликвидности
            result.П5 = SafeDivide(data.Код1210 + data.Код1220 + data.Код1230, data.Код1200 - data.Код1500); // Коэффициент маневренности функционирующего капитала
            result.П6 = SafeDivide(data.Код1200, data.Код1700); // Доля оборотных средств в активах
            result.П7 = SafeDivide(data.Код1300 - data.Код1100, data.Код1200); // Коэффициент обеспеченности собственными средствами
            result.П8 = SafeDivide(data.Код1200 + data.Код1100, data.Код1500 + data.Код1400); // Коэффициент обеспеченности обязательств активами

            // Общая оценка кредитного риска (по моделям)
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