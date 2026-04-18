namespace CryptoAlert.PriceCollector.Services;

public interface ICryptoPriceProvider
{
    Task<decimal?> GetPriceAsync(string assetId, string vsCurrency, CancellationToken cancellationToken);
}