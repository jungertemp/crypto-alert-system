using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Api.Models.Responses;

public class AlertResponse
{
    public int Id { get; set; }

    public string AssetId { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public decimal TargetPrice { get; set; }

    public AlertConditionType ConditionType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}