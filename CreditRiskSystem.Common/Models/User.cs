using System;
using System.Collections.Generic;

namespace CreditRiskSystem.Common.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<FinancialData> FinancialDatas { get; set; }
    }
}