using CryptoAlert.Database.Entities;

namespace CryptoAlert.Notifications.Services;

public interface IPriceHistoryService
{
    Task SaveAsync(PriceHistory priceHistory, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PriceHistory>> GetByAssetAsync(
        string assetId,
        int take,
        CancellationToken cancellationToken);
}