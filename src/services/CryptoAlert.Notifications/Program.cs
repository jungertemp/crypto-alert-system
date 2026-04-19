using CryptoAlert.Database;
using CryptoAlert.Notifications.Services;
using CryptoAlert.Notifications.Workers;
using CryptoAlert.SharedKernel.Options;

using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// bind RabbitMQ config
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

// DB (for next step)
builder.Services.AddDbContext<CryptoAlertDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// worker
builder.Services.AddScoped<IAlertEvaluationService, AlertEvaluationService>();
builder.Services.AddScoped<IPriceHistoryService, PriceHistoryService>();
builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();
host.Run();