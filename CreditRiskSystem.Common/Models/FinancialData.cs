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
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        // Коды из Бухгалтерского баланса (Форма 1)
        public double Код1100 { get; set; } // Внеоборотные активы
        public double Код1110 { get; set; } // Нематериальные активы
        public double Код1150 { get; set; } // Основные средства
        public double Код1200 { get; set; } // Оборотные активы
        public double Код1210 { get; set; } // Запасы
        public double Код1220 { get; set; } // НДС по приобретенным ценностям
        public double Код1230 { get; set; } // Дебиторская задолженность
        public double Код1240 { get; set; } // Финансовые вложения (краткосрочные)
        public double Код1250 { get; set; } // Денежные средства и денежные эквиваленты
        public double Код1260 { get; set; } // Прочие оборотные активы
        public double Код1300 { get; set; } // Капитал и резервы
        public double Код1370 { get; set; } // Нераспределенная прибыль (непокрытый убыток)
        public double Код1400 { get; set; } // Долгосрочные обязательства
        public double Код1500 { get; set; } // Краткосрочные обязательства
        public double Код1510 { get; set; } // Краткосрочные заемные обязательства
        public double Код1520 { get; set; } // Кредиторская задолженность
        public double Код1530 { get; set; } // Доходы будущих периодов
        public double Код1540 { get; set; } // Оценочные обязательства
        public double Код1550 { get; set; } // Прочие краткосрочные обязательства
        public double Код1600 { get; set; } // Баланс (валюта баланса, актив)
        public double Код1700 { get; set; } // Баланс (пассив)

        // Коды из Отчета о финансовых результатах (Форма 2)
        public double Код2100 { get; set; } // Валовая прибыль (убыток)
        public double Код2110 { get; set; } // Выручка
        public double Код2120 { get; set; } // Себестоимость продаж
        public double Код2200 { get; set; } // Прибыль (убыток) от продаж
        public double Код2210 { get; set; } // Коммерческие расходы
        public double Код2220 { get; set; } // Управленческие расходы
        public double Код2300 { get; set; } // Прибыль (убыток) до налогообложения
        public double Код2330 { get; set; } // Текущий налог на прибыль
        public double Код2350 { get; set; } // Прочие расходы
        public double Код2400 { get; set; } // Чистая прибыль (убыток)
    }
}
