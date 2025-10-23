# ✈️ Flight Service API

## 🚀 Установка

### ⚙️ Настройка базы данных

1. **Создайте базу данных в PostgreSQL:**

```sql
CREATE DATABASE flightservicedb;
```

2. **Обновите строку подключения** в `appsettings.json` или `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DatabaseConnection": "Host=localhost;Port=5432;Database=flightservicedb;Username=;Password="
  }
}
```

3. **Настройте JWT секрет** (опционально, по умолчанию уже установлен):

```json
{
  "JwtSettings": {
    "Secret": "",
    "Issuer": "FlightServiceAPI",
    "Audience": "FlightServiceAPI",
    "ExpirationMinutes": "60"
  }
}
```

### 🔨 Применение миграций

Перейдите в директорию проекта Presentation и выполните команды:

```bash
cd FlightServiceAPI.Presentation

# Добавить миграцию (если еще не добавлена)
dotnet ef migrations add InitialCreate --project ../FlightServiceAPI.Infrastructure

# Применить миграции к базе данных
dotnet ef database update --project ../FlightServiceAPI.Infrastructure
```

> ⚠️ **Важно!** Приложение автоматически применит миграции при первом запуске и создаст тестовых пользователей.

---

<div align="center">
Made with ❤️ for Air Astana
</div>
