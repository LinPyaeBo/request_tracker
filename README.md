# Request Tracker

A small service-request tracking application built with ASP.NET Core 10, C#, PostgreSQL, Entity Framework Core, and a plain HTML/CSS/JavaScript interface.

## Current features

- Create a service request with a title, description, and optional requester.
- List requests newest first and view an individual request through the API.
- Change a request status: Pending, InProgress, or Completed.
- Validate required fields and maximum input lengths.
- Store request data in PostgreSQL.
- Unit tests for request input validation.

## Requirements

- .NET 10 SDK
- PostgreSQL 17 or a compatible PostgreSQL server

## Run locally

1. Create a PostgreSQL database named `request_tracker`.
2. Set the connection string as an environment variable. Do not commit credentials.

PowerShell example:

```powershell
$env:ConnectionStrings__Default = "Host=localhost;Port=5432;Database=request_tracker;Username=YOUR_USER;Password=YOUR_PASSWORD"
dotnet run --project src/RequestTracker.Api
```

3. Open the local URL printed by ASP.NET Core.

The database schema must be created before the first run. The current version does not run EF Core migrations automatically.

## Run tests

```powershell
dotnet test tests/RequestTracker.Api.Tests/RequestTracker.Api.Tests.csproj
```

## API

- `GET /health`
- `GET /api/requests`
- `GET /api/requests/{id}`
- `POST /api/requests`
- `PUT /api/requests/{id}/status`

Example request body:

```json
{
  "title": "Printer issue",
  "description": "The office printer is offline.",
  "requestedBy": "Aye Aye"
}
```

## Scope boundaries

Authentication, custom roles, attachments, notifications, and configurable workflows are intentionally excluded from this first version.

## AI coding reflection

This repository is also being used to record a real coding exercise. Any reflection about AI assistance must be based on actual code, observed behavior, and issues the student personally identifies. No bugs or student discoveries should be invented for the assignment.
