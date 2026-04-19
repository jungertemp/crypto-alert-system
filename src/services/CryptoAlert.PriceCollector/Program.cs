using CryptoAlert.PriceCollector.Options;
using CryptoAlert.PriceCollector.Services;
using CryptoAlert.PriceCollector.Workers;
using CryptoAlert.SharedKernel.Options;

using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<CoinGeckoOptions>(builder.Configuration.GetSection("CoinGecko"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddHttpClient<ICryptoPriceProvider, CoinGeckoPriceProvider>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CoinGeckoOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<PriceCollectorWorker>();

var host = builder.Build();
host.Run();