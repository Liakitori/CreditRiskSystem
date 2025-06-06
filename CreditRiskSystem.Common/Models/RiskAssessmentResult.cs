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
        public DateTime CalculatedAt { get; set; }
        public FinancialData FinancialData { get; set; }
    }
}
