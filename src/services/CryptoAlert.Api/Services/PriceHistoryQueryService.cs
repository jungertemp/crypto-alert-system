using CryptoAlert.Api.Models.Responses;
using CryptoAlert.Database;

using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Api.Services;

public class PriceHistoryQueryService : IPriceHistoryQueryService
{
    private readonly CryptoAlertDbContext _dbContext;

    public PriceHistoryQueryService(CryptoAlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PriceHistoryPointResponse>> GetHistoryAsync(
        string assetId,
        int hours,
        CancellationToken cancellationToken)
    {
        var normalizedAssetId = assetId.Trim().ToLowerInvariant();
        var normalizedHours = hours <= 0 ? 24 : hours;

        var from = DateTime.UtcNow.AddHours(-normalizedHours);

        var result = await _dbContext.PriceHistories
            .AsNoTracking()
            .Where(x => x.AssetId == normalizedAssetId && x.CapturedAt >= from)
            .OrderBy(x => x.CapturedAt)
            .Select(x => new PriceHistoryPointResponse
            {
                Price = x.Price,
                CapturedAt = x.CapturedAt
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}