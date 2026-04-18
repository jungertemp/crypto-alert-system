using System.Text;
using System.Text.Json;
using CryptoAlert.Contracts.Events;
using CryptoAlert.Database.Entities;
using CryptoAlert.Notifications.Services;
using CryptoAlert.PriceCollector.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CryptoAlert.Notifications.Workers;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;

    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IOptions<RabbitMqOptions> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: _options.ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: string.Empty,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<PriceUpdatedEvent>(json);

                if (message is null)
                {
                    _logger.LogWarning("Failed to deserialize PriceUpdatedEvent");
                    return;
                }

                _logger.LogInformation(
                    "Received price update: {Symbol} = {Price}",
                    message.Symbol,
                    message.Price);

                if (stoppingToken.IsCancellationRequested)
                    return;

                using var scope = _serviceProvider.CreateScope();
                var priceHistoryService = scope.ServiceProvider.GetRequiredService<IPriceHistoryService>();
                var alertEvaluationService = scope.ServiceProvider.GetRequiredService<IAlertEvaluationService>();

                await priceHistoryService.SaveAsync(new PriceHistory
                {
                    AssetId = message.AssetId,
                    Symbol = message.Symbol,
                    Price = message.Price,
                    Currency = message.Currency,
                    CapturedAt = message.CheckedAt
                }, stoppingToken);

                await alertEvaluationService.EvaluateAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}