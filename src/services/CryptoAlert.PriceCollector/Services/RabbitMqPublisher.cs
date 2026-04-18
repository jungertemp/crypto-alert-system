using System.Text;
using System.Text.Json;
using CryptoAlert.Contracts.Events;
using CryptoAlert.PriceCollector.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CryptoAlert.PriceCollector.Services;

public sealed class RabbitMqPublisher : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _initialized;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
            return;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: _options.ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _initialized = true;
    }

    public async Task PublishAsync(PriceUpdatedEvent message, CancellationToken cancellationToken)
    {
        if (!_initialized || _channel is null)
            throw new InvalidOperationException("RabbitMqPublisher is not initialized.");

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: string.Empty,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}