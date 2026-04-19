# 🚀 Crypto Alert System

A full-stack crypto price monitoring system with real-time processing, alerts, and historical data visualization.

---

## 🧠 Overview

This project monitors cryptocurrency prices, stores historical data, evaluates user alerts, and displays charts in a web UI.

---

## 🏗 Architecture

Microservices-based architecture:

- API – REST endpoints (alerts, history)
- PriceCollector – fetches crypto prices (CoinGecko)
- Notifications – processes alerts from events
- RabbitMQ – event bus
- MSSQL – persistence
- Frontend (React + Vite) – UI with chart

PriceCollector → RabbitMQ → Notifications → DB
↓
API → Frontend


---

## 🧰 Tech Stack

- .NET 8 (ASP.NET Core, Worker Services)
- Entity Framework Core (MSSQL)
- RabbitMQ
- React (Vite + Recharts)
- Docker + Docker Compose

---

## ⚙️ Running the project (Docker)

### 1. Build & run

```bash
docker compose -f docker/docker-compose.yml up --build


| Service     | URL                                                            |
| ----------- | -------------------------------------------------------------- |
| Frontend    | [http://localhost:3000](http://localhost:3000)                 |
| API         | [http://localhost:8080/swagger](http://localhost:8080/swagger) |
| RabbitMQ UI | [http://localhost:15672](http://localhost:15672)               |

Key features
-Fetch crypto prices every ~10 seconds
-Store price history (time-series)
-Display chart in UI
-Create price alerts
-Event-driven processing via RabbitMQ
-Fully dockerized environment

📦 Environment Notes

Inside Docker:

API connects to DB via mssql
Services connect to RabbitMQ via rabbitmq
Frontend calls API via http://localhost:8080


⚠️ Known limitations
No auth (yet)
Alerts not sent to external channels (email/telegram)
No retry/backoff polish for all services
No aggregation for historical data


🛣 Next steps

Planned improvements:

 Add user authentication (Google OAuth)
 Add alert notifications (email / Telegram)
 Improve UI/UX
 Add price aggregation (1m / 5m / 1h)
 Add caching layer (Redis)
 Improve retry & resiliency (RabbitMQ, DB)
 Use env variables instead of hardcoded API URL


 🧪 Development notes
Frontend API URL currently hardcoded:
const API_URL = "http://localhost:8080";
CORS allows:
http://localhost:3000
http://localhost:5173



Built as a learning + portfolio project focused on:
microservices
async/event-driven systems
real-time UI