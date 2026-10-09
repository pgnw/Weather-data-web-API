# Weather Data REST API

A RESTful API built with **C# and ASP.NET** for storing, managing, and querying weather data in **MongoDB**.

The API supports CRUD operations, flexible weather-data filtering, API-key authentication with role-based access control, and interactive API documentation through Swagger.

## Features

- Create, retrieve, update, and delete weather records
- Query weather data by:
  - Date
  - Location
  - Temperature range
- API-key authentication for protected endpoints
- Role-based access control for different levels of API access
- MongoDB persistence
- Swagger / OpenAPI documentation
- Layered application structure separating API, business logic, and data access concerns

## Architecture

The project uses a layered structure to keep HTTP handling, application logic, authentication, and database access separated.

```text
HTTP Request
     │
     ▼
 Middleware
     │
     ▼
 Controllers
     │
     ▼
  Services
     │
     ▼
Repositories
     │
     ▼
  MongoDB
```

### Project Structure

```text
Weather-data-web-API/
├── Controllers/     # REST API endpoints and request handling
├── Middleware/      # Request pipeline and authentication
├── Models/          # Application and database models
├── Repositories/    # MongoDB data-access layer
├── Services/        # Application logic
├── Settings/        # Application and database configuration
├── docs/            # Generated API documentation
├── Program.cs       # Application startup and dependency configuration
└── appsettings.json
```

## Tech Stack

| Technology | Purpose |
|---|---|
| C# | Application language |
| ASP.NET | REST API framework |
| MongoDB | Weather data persistence |
| Swagger / OpenAPI | API documentation and endpoint testing |
| Git / GitHub | Source control |

## Getting Started

### Prerequisites

You will need:

- A compatible .NET SDK
- MongoDB running locally or a MongoDB connection string
- Git

### Clone the Repository

```bash
git clone https://github.com/pgnw/Weather-data-web-API.git
cd Weather-data-web-API
```

### Restore Dependencies

```bash
dotnet restore
```

### Configure MongoDB

Configure the MongoDB connection settings in `appsettings.json` or `appsettings.Development.json` for your environment.

Avoid committing production credentials or API keys to source control.

### Run the API

```bash
dotnet run
```

ASP.NET will display the local URLs the application is listening on when it starts.

## Authentication

Protected endpoints use **API keys** for authentication.

API keys are associated with access levels, allowing endpoint permissions to be restricted based on the role of the authenticated user.

This provides a simple authentication and authorization layer without exposing protected operations publicly.

## API Documentation

Swagger/OpenAPI documentation is available for exploring the API and testing endpoints interactively.

**Hosted documentation:**  
https://pgnw.github.io/Weather-data-web-API/

The Swagger documentation describes the available routes, request parameters, request bodies, response models, and authentication requirements.

## Querying Weather Data

Weather records can be queried using filters such as:

- Location
- Date
- Temperature range

These filters allow clients to retrieve specific subsets of weather observations rather than loading the complete dataset.

For the complete endpoint definitions and supported query parameters, see the Swagger documentation.

## License

This project is available under the license included in [`LICENSE.md`](LICENSE.md).
