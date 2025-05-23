using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditRiskSystem.Common.Models
{
    public class FinancialData
    {
        public Guid Id { get; set; }
        public double WorkingCapital { get; set; } // Оборотный капитал
        public double TotalAssets { get; set; } // Общие активы
        public double RetainedEarnings { get; set; } // Нераспределенная прибыль
        public double EBIT { get; set; } // Прибыль до вычета налогов и процентов
        public double MarketValueOfEquity { get; set; } // Рыночная стоимость капитала
        public double TotalLiabilities { get; set; } // Общие обязательства
        public double Revenue { get; set; } // Выручка
        public DateTime CreatedAt { get; set; }
    }
}
