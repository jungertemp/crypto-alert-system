# Crypto Alert System

Microservices-based system for tracking cryptocurrency prices and notifying users when conditions are met.

## Tech stack
- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- RabbitMQ
- Redis
- Docker
- Swagger

## Architecture (MVP)
- CryptoAlert.Api → manages alerts
- CryptoAlert.PriceCollector → fetches prices
- CryptoAlert.Notifications → evaluates alerts
- CryptoAlert.Dispatcher → sends notifications (Telegram planned)
- CryptoAlert.Logger → logs events

## Status
🚧 Initial project structure created  
🚧 EF Core setup in progress  

## Run (soon)
```bash
docker compose up --build