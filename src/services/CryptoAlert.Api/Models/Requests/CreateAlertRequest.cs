using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Api.Models.Requests;

public class CreateAlertRequest
{
    public string AssetId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public decimal TargetPrice { get; set; }
    public AlertConditionType ConditionType { get; set; }
}