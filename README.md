# PortfolioProject - Habit Tracker Pro

Портфельный проект на C# для GitHub и собеседований: API + вау-дашборд с живой статистикой привычек.

## Что реализовано
- ASP.NET Core Minimal API (.NET 10)
- Современный фронт на `HTML/CSS/JS` в `wwwroot`
- Аналитика: streak, weekly target, статистика за 7 дней, weekly activity chart
- Сервисный слой `Contracts / Models / Services`
- Юнит-тесты на `xUnit`
- Dockerfile для контейнерного запуска

## Архитектура
- `PortfolioProject.Api` - backend + dashboard
  - `Contracts` - DTO для запросов/ответов
  - `Models` - доменные сущности
  - `Services` - бизнес-логика
  - `wwwroot` - UI (дашборд)
- `PortfolioProject.Tests` - тесты сервиса

## Локальный запуск
```bash
cd PortfolioProject
dotnet run --project PortfolioProject.Api
```

После старта:
- Dashboard: `http://localhost:5151/`
- OpenAPI JSON: `http://localhost:5151/openapi/v1.json`
- Health check: `http://localhost:5151/api/health`

## API эндпоинты
- `GET /api/habits`
- `POST /api/habits`
- `POST /api/habits/{id}/complete`
- `DELETE /api/habits/{id}`
- `GET /api/habits/stats`
- `GET /api/habits/activity/weekly`
- `GET /api/health`

## Пример запроса
`POST /api/habits`
```json
{
  "name": "Practice system design",
  "category": "Interview",
  "weeklyTarget": 4
}
```

## Тесты
```bash
dotnet test
```

## Docker
```bash
docker build -t portfolio-habit-tracker -f PortfolioProject.Api/Dockerfile .
docker run --rm -p 8080:8080 portfolio-habit-tracker
```
