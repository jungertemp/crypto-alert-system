using CryptoAlert.Database;
using CryptoAlert.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Notifications.Services;

public class PriceHistoryService : IPriceHistoryService
{
    private readonly CryptoAlertDbContext _dbContext;
    private readonly ILogger<PriceHistoryService> _logger;

    public PriceHistoryService(
        CryptoAlertDbContext dbContext,
        ILogger<PriceHistoryService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SaveAsync(PriceHistory priceHistory, CancellationToken cancellationToken)
    {
        _dbContext.PriceHistories.Add(priceHistory);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PriceHistory>> GetByAssetAsync(
        string assetId,
        int take,
        CancellationToken cancellationToken)
    {
        var result = await _dbContext.PriceHistories
            .AsNoTracking()
            .Where(x => x.AssetId == assetId)
            .OrderByDescending(x => x.CapturedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        return result;
    }
}