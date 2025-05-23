
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
        /*Если потребуется можно воскресить
        также
        dotnet ef migrations remove
        dotnet ef migrations add InitialCreate
        dotnet ef database update
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RiskAssessmentResult>()
                .HasOne<FinancialData>()
                .WithMany()
                .HasForeignKey(r => r.FinancialDataId);
        }*/
    }
}
