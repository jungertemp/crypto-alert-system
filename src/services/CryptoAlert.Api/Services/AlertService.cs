using CryptoAlert.Api.Models.Requests;
using CryptoAlert.Api.Models.Responses;
using CryptoAlert.Database;
using CryptoAlert.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Api.Services;

public class AlertService : IAlertService
{
    private static readonly Guid DemoUserId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly CryptoAlertDbContext _dbContext;

    public AlertService(CryptoAlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken cancellationToken)
    {
        var entity = new PriceAlert
        {
            UserId = DemoUserId,
            AssetId = request.AssetId,
            Symbol = request.Symbol.Trim().ToUpperInvariant(),
            TargetPrice = request.TargetPrice,
            ConditionType = request.ConditionType,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.PriceAlerts.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<AlertResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.PriceAlerts
            .AsNoTracking()
            .Where(x => x.Id == id && x.UserId == DemoUserId)
            .Select(x => new AlertResponse
            {
                Id = x.Id,
                AssetId = x.AssetId,
                Symbol = x.Symbol,
                TargetPrice = x.TargetPrice,
                ConditionType = x.ConditionType,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AlertResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var collection = await _dbContext.PriceAlerts
            .AsNoTracking()
            .Where(x => x.UserId == DemoUserId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AlertResponse
            {
                Id = x.Id,
                AssetId = x.AssetId,
                Symbol = x.Symbol,
                TargetPrice = x.TargetPrice,
                ConditionType = x.ConditionType,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return collection;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var alertEntity = await _dbContext.PriceAlerts
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == DemoUserId, cancellationToken);

        if (alertEntity is null)
            return false;

        _dbContext.PriceAlerts.Remove(alertEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<AlertResponse?> DeactivateAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PriceAlerts
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == DemoUserId, cancellationToken);

        if (entity is null)
            return null;

        if (!entity.IsActive)
            return Map(entity);

        entity.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private static AlertResponse Map(PriceAlert entity)
    {
        return new AlertResponse
        {
            Id = entity.Id,
            AssetId = entity.AssetId,
            Symbol = entity.Symbol,
            TargetPrice = entity.TargetPrice,
            ConditionType = entity.ConditionType,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}