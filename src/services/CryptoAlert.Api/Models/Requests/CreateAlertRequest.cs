using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Api.Models.Requests;

public record CreateAlertRequest
{
    public string Symbol { get; set; } = null!;
    public decimal TargetPrice { get; set; }
    public AlertConditionType ConditionType { get; set; }
}