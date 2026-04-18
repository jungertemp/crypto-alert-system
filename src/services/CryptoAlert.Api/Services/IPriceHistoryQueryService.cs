using CryptoAlert.Api.Models.Responses;

namespace CryptoAlert.Api.Services;

public interface IPriceHistoryQueryService
{
    Task<IReadOnlyCollection<PriceHistoryPointResponse>> GetHistoryAsync(
        string assetId,
        int hours,
        CancellationToken cancellationToken);
}