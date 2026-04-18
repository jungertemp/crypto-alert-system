using CryptoAlert.Contracts.Events;

namespace CryptoAlert.Notifications.Services;

public interface IAlertEvaluationService
{
    Task EvaluateAsync(PriceUpdatedEvent message, CancellationToken cancellationToken);
}