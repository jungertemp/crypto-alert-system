using Microsoft.EntityFrameworkCore;
using CryptoAlert.Database.Entities;

namespace CryptoAlert.Database;

public class CryptoAlertDbContext : DbContext
{
    public CryptoAlertDbContext(DbContextOptions<CryptoAlertDbContext> options)
        : base(options)
    {
    }

    public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PriceAlert>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.TargetPrice)
                .HasColumnType("decimal(18,4)");
        });
    }
}