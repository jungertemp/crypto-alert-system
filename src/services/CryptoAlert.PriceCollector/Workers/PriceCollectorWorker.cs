using CryptoAlert.Contracts.Events;
using CryptoAlert.PriceCollector.Services;

namespace CryptoAlert.PriceCollector.Workers;

public class PriceCollectorWorker : BackgroundService
{
    private readonly ILogger<PriceCollectorWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqPublisher _publisher;

    public PriceCollectorWorker(
        ILogger<PriceCollectorWorker> logger,
        IServiceProvider serviceProvider,
        RabbitMqPublisher publisher)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _publisher.InitializeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var provider = scope.ServiceProvider.GetRequiredService<ICryptoPriceProvider>();

                var price = await provider.GetPriceAsync("ethereum", "usd", stoppingToken);

                if (price is not null)
                {
                    var message = new PriceUpdatedEvent
                    {
                        AssetId = "ethereum",
                        Symbol = "ETH",
                        Price = price.Value,
                        Currency = "USD",
                        CheckedAt = DateTime.UtcNow
                    };

                    await _publisher.PublishAsync(message, stoppingToken);

                    _logger.LogInformation(
                        "Published price update for {Symbol}: {Price} {Currency}",
                        message.Symbol,
                        message.Price,
                        message.Currency);
                }
                else
                {
                    _logger.LogWarning("Price was not returned for ethereum.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while collecting and publishing crypto price");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _publisher.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}