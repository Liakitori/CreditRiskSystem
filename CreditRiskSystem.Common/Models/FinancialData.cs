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
        public double Код1200 { get; set; } // Оборотные активы (Форма 1)
        public double Код1300 { get; set; } // Капитал и резервы (Форма 1)
        public double Код1370 { get; set; } // Нераспределенная прибыль (Форма 1)
        public double Код1400 { get; set; } // Долгосрочные обязательства (Форма 1)
        public double Код1500 { get; set; } // Краткосрочные обязательства (Форма 1)
        public double Код1600 { get; set; } // Итого активов (Форма 1)
        public double Код2110 { get; set; } // Выручка (Форма 2)
        public double Код2300 { get; set; } // Прибыль до налогообложения (Форма 2)
        public double Код2330 { get; set; } // Текущий налог на прибыль (Форма 2)
        public double Код2400 { get; set; } // Чистая прибыль (Форма 2)
        public DateTime CreatedAt { get; set; }
    }
}
