using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditRiskSystem.Common.Models
{
    public class RiskAssessmentResult
    {
        public Guid FinancialDataId { get; set; }
        public double AltmanZScore { get; set; }
        public string RiskLevel { get; set; } // Низкий, Средний, Высокий
        public string Recommendations { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
