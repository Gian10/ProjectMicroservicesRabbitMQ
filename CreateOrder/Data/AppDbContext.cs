using Microsoft.EntityFrameworkCore;

namespace Order.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Orders> Pedidos { get; set; }
        public DbSet<Outbox> OutboxMessage { get; set; }  


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Orders>() 
                .Property(o => o.Valor)
                .HasPrecision(18, 2);


            // Configuração da Outbox
            modelBuilder.Entity<Outbox>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Content).IsRequired();
                entity.HasIndex(e => e.ProcessedAt);
            });
        }



    }
}
