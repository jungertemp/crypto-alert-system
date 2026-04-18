using CryptoAlert.Api.Models.Requests;
using CryptoAlert.Api.Models.Responses;
using CryptoAlert.Database;
using CryptoAlert.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Api.Services;

public class AlertService : IAlertService
{
    private readonly CryptoAlertDbContext _dbContext;

    public AlertService(CryptoAlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken cancellationToken)
    {
        var entity = new PriceAlert
        {
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Symbol = request.Symbol.Trim().ToUpperInvariant(),
            TargetPrice = request.TargetPrice,
            ConditionType = request.ConditionType,
        };

        _dbContext.PriceAlerts.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<AlertResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.PriceAlerts
            .Where(x => x.Id == id)
            .Select(x => new AlertResponse
            {
                Id = x.Id,
                Symbol = x.Symbol,
                TargetPrice = x.TargetPrice,
                ConditionType = x.ConditionType,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            }).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AlertResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var collection = await _dbContext.PriceAlerts
            .Where(x => x.IsActive)
            .Select(x => new AlertResponse
            {
                Id = x.Id,
                Symbol = x.Symbol,
                TargetPrice = x.TargetPrice,
                ConditionType = x.ConditionType,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            }).ToListAsync(cancellationToken);
        
        return collection;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var alterEntity = await _dbContext.PriceAlerts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (alterEntity == null)
        {
            return false;
        }
        
        _dbContext.PriceAlerts.Remove(alterEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<AlertResponse?> DeactivateAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PriceAlerts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

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
            Symbol = entity.Symbol,
            TargetPrice = entity.TargetPrice,
            ConditionType = entity.ConditionType,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}