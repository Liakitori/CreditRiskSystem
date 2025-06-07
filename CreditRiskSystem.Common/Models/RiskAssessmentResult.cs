using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditRiskSystem.Common.Models
{
    public class RiskAssessmentResult
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FinancialDataId { get; set; }
        public FinancialData FinancialData { get; set; }
        public DateTime CalculatedAt { get; set; }

        // Показатели кредитного риска
        public double AltmanZScore { get; set; }
        public string AltmanRiskLevel { get; set; }
        public double SpringateScore { get; set; }
        public string SpringateRiskLevel { get; set; }
        public double FulmerScore { get; set; }
        public string FulmerRiskLevel { get; set; }
        public double OhlsonOScore { get; set; }
        public double OhlsonProbability { get; set; }
        public double ZmijewskiScore { get; set; }
        public double ZmijewskiProbability { get; set; }
        public string OverallRiskAssessment { get; set; }

        // Рентабельность
        public double Р1 { get; set; } // Рентабельность объема продаж, %
        public double Р2 { get; set; } // Бухгалтерская рентабельность от обычной деятельности, %
        public double Р3 { get; set; } // Чистая рентабельность, %
        public double Р4 { get; set; } // Экономическая рентабельность, %
        public double Р5 { get; set; } // Рентабельность собственного капитала, %
        public double Р6 { get; set; } // Валовая рентабельность, %
        public double Р7 { get; set; } // Рентабельность реализованной продукции, %

        // Деловая активность
        public double ДА1 { get; set; } // Коэффициент общей оборачиваемости капитала, оборотов
        public double ДА2 { get; set; } // Коэффициент оборачиваемости оборотных средств, оборотов
        public double ДА3 { get; set; } // Коэффициент отдачи нематериальных активов, оборотов
        public double ДА4 { get; set; } // Фондоотдача, оборотов
        public double ДА5 { get; set; } // Коэффициент отдачи собственного капитала, оборотов
        public double ДА6 { get; set; } // Коэффициент оборачиваемости средств в расчетах, оборотов
        public double ДА7 { get; set; } // Коэффициент оборачиваемости кредиторской задолженности, оборотов
        public double ДА8 { get; set; } // Оборачиваемость материальных средств, дней
        public double ДА9 { get; set; } // Оборачиваемость денежных средств, дней
        public double ДА10 { get; set; } // Срок погашения дебиторской задолженности, дней
        public double ДА11 { get; set; } // Срок погашения кредиторской задолженности, дней

        // Финансовая устойчивость
        public double ФУ1 { get; set; } // Коэффициент капитализации
        public double ФУ2 { get; set; } // Собственный капитал в обороте
        public double ФУ3 { get; set; } // Коэффициент обеспеченности запасов собственными источниками
        public double ФУ4 { get; set; } // Коэффициент автономии
        public double ФУ5 { get; set; } // Коэффициент финансирования
        public double ФУ6 { get; set; } // Коэффициент финансовой устойчивости
        public double ФУ7 { get; set; } // Коэффициент маневренности
        public double ФУ8 { get; set; } // Коэффициент мобилизации

        // Платёжеспособность
        public double Л1 { get; set; } // Общий показатель платежеспособности
        public double П2 { get; set; } // Коэффициент абсолютной ликвидности
        public double П3 { get; set; } // Коэффициент быстрой ликвидности
        public double П4 { get; set; } // Коэффициент текущей ликвидности
        public double П5 { get; set; } // Коэффициент маневренности функционирующего капитала
        public double П6 { get; set; } // Доля оборотных средств в активах
        public double П7 { get; set; } // Коэффициент обеспеченности собственными оборотными средствами
        public double П8 { get; set; } // Коэффициент обеспеченности обязательств активами
    }
}
