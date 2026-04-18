using CryptoAlert.Contracts.Events;
using CryptoAlert.Database;
using CryptoAlert.SharedKernel.Enums;

using Microsoft.EntityFrameworkCore;

namespace CryptoAlert.Notifications.Services;

public class AlertEvaluationService : IAlertEvaluationService
{
    private readonly CryptoAlertDbContext _dbContext;
    private readonly ILogger<AlertEvaluationService> _logger;

    public AlertEvaluationService(
        CryptoAlertDbContext dbContext,
        ILogger<AlertEvaluationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task EvaluateAsync(PriceUpdatedEvent message, CancellationToken cancellationToken)
    {
        var activeAlerts = await _dbContext.PriceAlerts
            .AsNoTracking()
            .Where(pa => pa.Symbol == message.Symbol && pa.IsActive)
            .Select(pa => new
            {
                pa.Id,
                pa.TargetPrice,
                pa.ConditionType
            })
            .ToListAsync(cancellationToken);

        foreach (var alert in activeAlerts)
        {
            var isMatched = alert.ConditionType switch
            {
                AlertConditionType.GreaterThan => message.Price > alert.TargetPrice,
                AlertConditionType.LessThan => message.Price < alert.TargetPrice,
                _ => false
            };

            if (!isMatched)
                continue;

            _logger.LogInformation(
                "Matched alert {AlertId} for {Symbol}. Current price: {CurrentPrice}, target: {TargetPrice}, condition: {ConditionType}",
                alert.Id,
                message.Symbol,
                message.Price,
                alert.TargetPrice,
                alert.ConditionType);

            // next step: save notification or publish alert-triggered event
        }
    }
}