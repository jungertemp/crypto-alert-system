using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Database.Entities;

public class PriceAlert
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public decimal TargetPrice { get; set; }

    public AlertConditionType ConditionType { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}