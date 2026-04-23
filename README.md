# Literary Collection Management

**Languages:** **English** • [Português (Brasil)](./README.pt-BR.md)

> System for querying, registering, and maintaining genres, authors, and books.

---

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-17-DD0031?style=flat-square&logo=angular)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)
![EF Core](https://img.shields.io/badge/EF_Core-8.x-512BD4?style=flat-square)
![NgRx](https://img.shields.io/badge/NgRx-17-BA2BD2?style=flat-square)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4.x-38BDF8?style=flat-square&logo=tailwindcss)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker)
[![MIT License](https://img.shields.io/badge/License-MIT-teal.svg)](./LICENSE)

---

## Table of Contents

- [About the Project](#about-the-project)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Running with Docker (recommended)](#running-with-docker-recommended)
- [Setup and Run — Backend (without Docker)](#setup-and-run--backend-without-docker)
- [Setup and Run — Frontend (without Docker)](#setup-and-run--frontend-without-docker)
- [Running the Tests](#running-the-tests)
- [Styling — Angular Material + Tailwind CSS v4](#styling--angular-material--tailwind-css-v4)
- [API Test Collections](#api-test-collections)
- [API Documentation](#api-documentation)
- [Repository Structure](#repository-structure)
- [Known Issues](#known-issues)
- [License](#license)

---

## About the Project

**Gestão Acervo** is a full-stack application for managing a bibliographic collection, composed of:

- **Acervo.API** — A .NET 8 REST API (Minimal API) with route versioning, Swagger documentation, and standardized responses.
- **acervo-web** — An Angular 17 SPA using NgRx for state management and a hybrid styling approach: **Angular Material** components + **Tailwind CSS v4** utility classes.

### Core Business Rules

- A **Genre** can have N books; an **Author** can have N books.
- Each **Book** belongs to exactly one Author and one Genre.
- **ISBN** and **Author's email** are unique across the system.
- Authors and Genres with linked books **cannot be deleted**.

---

## Architecture

```
acervo-web  (Angular 17 + NgRx)
        │  HTTP/REST
Acervo.API  (.NET 8 Minimal API)
        │  EF Core
   SQL Server 2022
```

The backend follows **Clean Architecture** split into four projects:

| Project                 | Responsibility                                                        |
| ----------------------- | --------------------------------------------------------------------- |
| `Acervo.Domain`         | Entities, IRepository interfaces, Result pattern                      |
| `Acervo.Infrastructure` | DbContext, Repositories, Migrations                                   |
| `Acervo.Application`    | IService interfaces, Services, DTOs, ViewModels, Validators, Mappings |
| `Acervo.API`            | Endpoints (Minimal API), Configuration, AppSettings                   |
| `Acervo.Tests`          | Unit Tests (xUnit + Moq)                                              |

### Context Diagram

![Context Diagram - Gestão Acervo](./out/docs/C4/Context/Context.png)

---

## Prerequisites

### With Docker (recommended)

| Tool           | Minimum version | Check                    |
| -------------- | --------------- | ------------------------ |
| Docker Engine  | 24.x            | `docker --version`       |
| Docker Compose | 2.x (plugin)    | `docker compose version` |
| Git            | 2.x             | `git --version`          |

### Without Docker (local execution)

| Tool            | Minimum version | Download                                 |
| --------------- | --------------- | ---------------------------------------- |
| .NET SDK        | 8.0             | https://dotnet.microsoft.com/download    |
| Node.js         | 20.x LTS[^node] | https://nodejs.org                       |
| Angular CLI     | 17.x            | `npm install -g @angular/cli`            |
| SQL Server 2022 | 2022            | https://www.microsoft.com/sql-server     |
| EF Core CLI     | 8.x             | `dotnet tool install --global dotnet-ef` |

[^node]: Angular 17 officially supports Node 18.13+ and 20.9+. Node **20** is required for the Docker build because `npm 9` (bundled with Node 18) does not correctly resolve `libc: musl` in optional dependencies on Alpine, breaking Tailwind v4's Oxide native binding. Outside Docker, Node 18.13+ works fine for `ng serve`.

---

## Running with Docker (recommended)

The fastest way to run the full project — without installing .NET, Node, or SQL Server on your machine.

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/GestaoAcervo.git
cd gestaoacervo
```

### 2. Configure the environment variables

```bash
cp .env.example .env
```

Edit the `.env` file if you want to change the SQL Server password:

```env
SA_PASSWORD=Acervo@Dev2024!
```

> **Requirement:** minimum 8 characters with uppercase letters, lowercase letters, numbers, and a special symbol.

### 3. Start the whole stack

```bash
docker compose up --build
```

Compose will automatically:

1. Start **SQL Server 2022** and wait for the healthcheck to pass
2. Start the **Acervo.API** and apply **EF Core Migrations** automatically
3. Start the **Angular Frontend** via Nginx

### 4. Access the services

| Service       | URL                           |
| ------------- | ----------------------------- |
| Frontend      | http://localhost:4200         |
| REST API      | http://localhost:5000/api/v1  |
| Swagger UI    | http://localhost:5000/swagger |
| SQL Server    | `localhost,1433` / user: `sa` |

### Useful commands

```bash
# Run in the background
docker compose up -d

# Follow logs in real time
docker compose logs -f

# Stop containers (preserves data)
docker compose stop

# Remove containers and wipe database data
docker compose down -v
```

---

## Setup and Run — Backend (without Docker)

### 1. Clone and enter the directory

```bash
git clone https://github.com/<your-username>/GestaoAcervo.git
cd gestaoacervo/backend/Acervo.API
```

### 2. Configure the connection string

Edit `Acervo.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AcervoDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### 3. Run the Migrations

```bash
dotnet ef database update \
  --project Acervo.Infrastructure \
  --startup-project Acervo.API
```

### 4. Run the API

```bash
cd Acervo.API
dotnet run --environment Development
```

Available at `http://localhost:5000` | Swagger: `http://localhost:5000/swagger`

---

## Setup and Run — Frontend (without Docker)

```bash
cd gestao-acervo/frontend/acervo-web
npm install
ng serve
```

Available at `http://localhost:4200`

---

## Running the Tests

### Backend (xUnit + Moq)

```bash
# From the solution root
dotnet test

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend (Jasmine + Karma)

```bash
cd frontend/acervo-web

# Single run
ng test --watch=false

# Watch mode (development)
ng test
```

---

## Styling — Angular Material + Tailwind CSS v4

The frontend uses a **hybrid styling strategy**:

- **Angular Material 17** — UI components (`mat-toolbar`, `mat-table`, `mat-form-field`, dialogs, etc.) and the `deeppurple-amber` prebuilt theme (light).
- **Tailwind CSS v4** — utility classes for layout, spacing, flex/grid, and responsive breakpoints.

Tailwind v4 and Material coexist because Tailwind's Preflight is skipped (it would otherwise override Material's resets), and design tokens (`@theme`) mirror the Material palette so both engines stay visually aligned.

### Setup at a glance

| File                  | Purpose                                                         |
| --------------------- | --------------------------------------------------------------- |
| `src/tailwind.css`    | Tailwind imports (`theme` + `utilities`) and `@theme` tokens    |
| `src/styles.scss`     | Angular Material setup + global `html`/`body` rules only        |
| `.postcssrc.json`     | Registers the `@tailwindcss/postcss` plugin for Angular CLI     |
| `angular.json`        | Lists `src/tailwind.css` in `architect.build.options.styles[]`  |
| `package.json`        | `tailwindcss`, `@tailwindcss/postcss`, `postcss` as devDeps     |

### Installation (already applied)

```bash
cd frontend/acervo-web
npm install -D tailwindcss @tailwindcss/postcss postcss --legacy-peer-deps
```

> `--legacy-peer-deps` is required because Angular 17's `@angular-devkit/build-angular` declares a soft `peerOptional` on Tailwind v2/v3. PostCSS integration is unaffected at runtime.

---

## API Test Collections

The `./docs/collections` directory contains a **JSON** collection for use with Insomnia and another for Postman.

---

## API Documentation

With the API running, open the **Swagger UI**:

```
http://localhost:5000/swagger
```

### Available endpoints

| Resource | Base URL          |
| -------- | ----------------- |
| Authors  | `/api/v1/autores` |
| Genres   | `/api/v1/generos` |
| Books    | `/api/v1/livros`  |

### Request Example — Create Book

```http
POST /api/v1/livros
Content-Type: application/json

{
  "titulo": "Clean Code",
  "isbn": "9780132350884",
  "anoPublicacao": 2008,
  "autorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "generoId": "7fa12c48-1234-4562-b3fc-9d874f22bcd1"
}
```

### Response Example — Success

```json
{
  "success": true,
  "message": "Livro criado com sucesso.",
  "data": {
    "id": "a1b2c3d4-...",
    "titulo": "Clean Code",
    "isbn": "9780132350884",
    "anoPublicacao": 2008,
    "autorNome": "Robert C. Martin",
    "generoNome": "Tecnologia"
  }
}
```

### Response Example — Business Error

```json
{
  "success": false,
  "message": "Não foi possível concluir a operação.",
  "errors": ["ISBN '9780132350884' já está cadastrado no sistema."]
}
```

> **Note:** API response messages are currently returned in Portuguese; the examples above reflect the real payloads.

## Repository Structure

```
gestaoacervo/
├── docker-compose.yml           → Orchestration for the 3 containers
├── .env.example                 → Environment variables template
├── .gitignore
├── README.md
├── LICENSE
|
├── docs/
|   ├── collections/             → Collections for testing the application
|
├── docker/
│   ├── backend/
│   │   ├── Dockerfile           → Multi-stage build for Acervo.API
│   │   └── .dockerignore
│   ├── frontend/
│   │   ├── Dockerfile           → Multi-stage build + Nginx
│   │   ├── nginx.conf           → Nginx config for Angular Router
│   │   └── .dockerignore
│   └── sqlserver/
│       ├── init-db.sql          → AcervoDB database creation
│       └── entrypoint.sh        → Container startup script
│
├── backend/
│   └── Acervo/                  → .NET 8 Solution
├── frontend/
│   └── acervo-web/              → Angular 17 SPA
```

---

## Known Issues

The `entrypoint.sh` file (`\docker\sqlserver\entrypoint.sh`) may be checked out with Windows-style line endings (CRLF, `\r\n`), while the Linux shell used by Docker expects LF (Unix, `\n`). Since this file is the entry point that initializes SQL Server in the container, CRLF line endings can break the database container startup. Open the file in Visual Studio Code and convert it from CRLF to LF if needed.

## License

This repository is licensed under the [MIT](./LICENSE) License.
