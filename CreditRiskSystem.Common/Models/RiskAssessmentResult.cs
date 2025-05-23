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
        public Guid Id { get; set; } // Первичный ключ
        public Guid FinancialDataId { get; set; } // Внешний ключ для связи с FinancialData
        public double AltmanZScore { get; set; }
        public string RiskLevel { get; set; } // Низкий, Средний, Высокий
        public string Recommendations { get; set; }
        public DateTime CalculatedAt { get; set; }
        public FinancialData FinancialData { get; set; } // Навигационное свойство
    }
}
