
using CreditRiskSystem.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditRiskSystem.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FinancialData> FinancialData { get; set; }
        public DbSet<RiskAssessmentResult> RiskAssessmentResults { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
