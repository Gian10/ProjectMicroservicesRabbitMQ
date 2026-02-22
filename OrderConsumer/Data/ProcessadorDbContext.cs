using Microsoft.EntityFrameworkCore;

namespace OrderConsumer.Data
{
    public class ProcessadorDbContext : DbContext
    {

        public ProcessadorDbContext(DbContextOptions<ProcessadorDbContext> options)
       : base(options)
        {
        }

        public DbSet<LogProcessamento> LogsProcessamento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogProcessamento>()
                .ToTable("LogsProcessamento");

            modelBuilder.Entity<LogProcessamento>()
                .Property(l => l.Valor)
                .HasPrecision(18, 2);
        }
    }
}
