using CryptoAlert.SharedKernel.Enums;

namespace CryptoAlert.Api.Models.Responses;

public record AlertResponse
{
    public int Id { get; init; }
    public string Symbol { get; init; } = null!;
    public decimal TargetPrice { get; init; }
    public AlertConditionType ConditionType { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}