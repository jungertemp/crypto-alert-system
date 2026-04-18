using CryptoAlert.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Database;

public class CryptoAlertDbContext : DbContext
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
    
    public CryptoAlertDbContext(DbContextOptions<CryptoAlertDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.GoogleSubjectId)
                .HasMaxLength(200);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.GoogleSubjectId)
                .IsUnique()
                .HasFilter("[GoogleSubjectId] IS NOT NULL");
        });

        modelBuilder.Entity<PriceAlert>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssetId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.TargetPrice)
                .HasColumnType("decimal(18,4)");

            entity.HasOne(x => x.User)
                .WithMany(x => x.PriceAlerts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.AssetId, x.IsActive });
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssetId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Price)
                .HasColumnType("decimal(18,8)");

            entity.HasIndex(x => new { x.AssetId, x.CapturedAt });
        });
    }
}