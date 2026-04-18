namespace CryptoAlert.Contracts.Events;

public class PriceUpdatedEvent
{
    public string AssetId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CheckedAt { get; set; }
}