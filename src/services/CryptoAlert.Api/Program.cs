using CryptoAlert.Api.Services;
using CryptoAlert.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<CryptoAlertDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IPriceHistoryQueryService, PriceHistoryQueryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("frontend");

// disable this for Docker HTTP setup for now
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();