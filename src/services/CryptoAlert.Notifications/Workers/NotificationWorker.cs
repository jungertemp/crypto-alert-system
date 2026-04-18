using System.Text;
using System.Text.Json;
using CryptoAlert.Contracts.Events;
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
    private readonly IAlertEvaluationService _alertEvaluationService;

    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IOptions<RabbitMqOptions> options,
        IAlertEvaluationService alertEvaluationService)
    {
        _logger = logger;
        _options = options.Value;
        _alertEvaluationService = alertEvaluationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // declare exchange
        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: _options.ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        // declare queue
        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        // bind queue to exchange
        await _channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: string.Empty,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<PriceUpdatedEvent>(json);

                if (message is null)
                {
                    _logger.LogWarning("Failed to deserialize message");
                    return;
                }

                _logger.LogInformation(
                    "Received price update: {Symbol} = {Price}",
                    message.Symbol,
                    message.Price);

                if (stoppingToken.IsCancellationRequested)
                    return;

                await _alertEvaluationService.EvaluateAsync(message, stoppingToken);
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