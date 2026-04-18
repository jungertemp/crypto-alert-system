using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Database.Entities;

public class PriceAlert
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string AssetId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public decimal TargetPrice { get; set; }
    public AlertConditionType ConditionType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AppUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? GoogleSubjectId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PriceAlert> PriceAlerts { get; set; } = new List<PriceAlert>();
}

public class PriceHistory
{
    public long Id { get; set; }

    public string AssetId { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public decimal Price { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}