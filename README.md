# Game of Life API

This is a .NET 9 Web API that simulates [Conway’s Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life), a cellular automaton where a grid of cells evolves according to a set of simple rules.

The API allows you to:
- Create and initialize boards
- Retrieve the state of a board
- Advance the board to the next generation
- Delete a board

## 📁 Project Structure

| Path                         | Description                                                       |
|------------------------------|-------------------------------------------------------------------|
| `GameOfLife.Api/`            | ASP.NET Core Web API project (e.g., Controllers, Program.cs)      |
| `GameOfLife.Business/`       | Business logic, use cases, entities and Domain Entities           |
| `GameOfLife.Infrastructure/` | Infrastructure implementations (e.g., Dependencies, repositories) |
| `GameOfLife.Tests/`          | Unit and integration test projects                                |
| `docker-compose.yml`         | Docker Compose setup for the API and PostgreSQL                   |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

---

## 🐳 Running the Project with Docker & Docker Compose

```bash
# 1. Clone the repository
git clone https://github.com/your-username/game-of-life-api.git
cd game-of-life-api

# 2. Build and start the API and PostgreSQL containers
docker-compose up --build
```
### ✅ Services
- #### 🧠 Game of Life API
  - Available at: https://localhost:7097/swagger
  - Swagger UI is enabled in development mode.

- #### 🐘 PostgreSQL Database (Accessible on port 5432)
    - Port: 5432
    - User: conway
    - Password g@m30fl1f3
    - Database: game_of_life

> ℹ️ Ensure that port 5432 is free on your machine, and that Docker is installed and running.
---

## 🧪 Running Tests

```
cd GameOfLife.Tests

# Run all tests
dotnet test
```
Tests are written using xUnit and cover use cases, controller, and other business logic classes.

---
## 📖 API Documentation
Swagger is available during development:

🔗 https://localhost:7097/swagger

JSON Spec: /swagger/v1/swagger.json

---
## 📡 BoardsController — Endpoints Overview
This controller handles all operations related to the game board.

> POST /boards
Creates a new board.

Request body: JSON with board dimensions and initial cell states.

Returns: Board ID and initial state.

Example Request:
```
{
  "grid": [
    [0, 1, 0, 0, 0],
    [0, 0, 1, 0, 0],
    [1, 1, 1, 0, 0],
    [0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0]
  ]
}
```

> GET /boards/{id}

Retrieves the current state of a specific board.

Path Parameter: id — ID of the board.

Returns: Current board matrix.

> GET /boards/{id}/next

Advances the board by one generation according to Conway’s rules.

- Path Parameter: id — ID of the board.
- Returns: New state after evolution.

Rules Implemented:

1. Any live cell with 2 or 3 live neighbors survives.
2. Any dead cell with exactly 3 live neighbors becomes alive.
3. All other live cells die in the next generation.

---

## 🛠️ Technologies
ASP.NET Core 9

Entity Framework Core

PostgreSQL

Docker + Docker Compose

Swagger / OpenAPI

xUnit (tests)