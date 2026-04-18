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
            .Where(pa => pa.Symbol == message.Symbol && pa.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var alert in activeAlerts)
        {
            var isMatched = alert.ConditionType switch
            {
                AlertConditionType.GreaterThan => message.Price > alert.TargetPrice,
                AlertConditionType.LessThan => message.Price < alert.TargetPrice,
                _ => false
            };

            if (isMatched && !alert.IsTriggered)
            {
                _logger.LogInformation(
                    "Triggered alert {AlertId} for {Symbol}. Current price: {CurrentPrice}, target: {TargetPrice}, condition: {ConditionType}",
                    alert.Id,
                    message.Symbol,
                    message.Price,
                    alert.TargetPrice,
                    alert.ConditionType);

                alert.IsTriggered = true;

                // later:
                // publish notification event
                // save notification history
            }
            else if (!isMatched && alert.IsTriggered)
            {
                _logger.LogInformation(
                    "Reset alert {AlertId} for {Symbol}. Current price: {CurrentPrice} no longer matches target {TargetPrice}",
                    alert.Id,
                    message.Symbol,
                    message.Price,
                    alert.TargetPrice);

                alert.IsTriggered = false;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}