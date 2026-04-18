using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using CryptoAlert.PriceCollector.Options;
using CryptoAlert.PriceCollector.Services;

public class CoinGeckoPriceProvider : ICryptoPriceProvider
{
    private readonly HttpClient _httpClient;
    private readonly CoinGeckoOptions _options;

    public CoinGeckoPriceProvider(
        HttpClient httpClient,
        IOptionsMonitor<CoinGeckoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.CurrentValue;
    }

    public async Task<decimal?> GetPriceAsync(
        string assetId,
        string vsCurrency,
        CancellationToken cancellationToken)
    {
        var url = $"simple/price?ids={assetId}&vs_currencies={vsCurrency}&x_cg_demo_api_key={_options.ApiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync
            <Dictionary<string, Dictionary<string, decimal>>>(cancellationToken: cancellationToken);

        return result?[assetId]?[vsCurrency];
    }
}