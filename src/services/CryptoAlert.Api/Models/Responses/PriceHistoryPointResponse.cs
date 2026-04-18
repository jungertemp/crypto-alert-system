namespace CryptoAlert.Api.Models.Responses;

public class PriceHistoryPointResponse
{
    public decimal Price { get; set; }

    public DateTime CapturedAt { get; set; }
}