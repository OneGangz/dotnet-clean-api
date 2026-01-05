# CleanAPI

A production-ready **.NET Clean Architecture API** designed to be simple, explicit, and boring in the right ways. This project exists to demonstrate clear separation of concerns, SOLID principles, and real-world API structure without framework magic or architectural cosplay.

If you’re looking for a flashy demo, this isn’t it. If you’re looking for something you can reason about at 2 a.m. when prod is on fire, you’re in the right place.

---

## What this project is

CleanAPI is a layered ASP.NET API that follows Clean Architecture principles:

* Business rules are independent of frameworks
* Infrastructure details are replaceable
* Dependencies always point inward
* Controllers are thin and boring

No shortcuts. No god classes. No "we’ll refactor later" lies.

---

## Architecture Overview

The solution is split into clear layers:

### 1. Domain

* Core business entities
* Business rules and invariants
* No dependencies on any other project

This layer knows nothing about databases, HTTP, JSON, or Entity Framework. It should survive a framework apocalypse.

### 2. Application

* Use cases and business workflows
* Interfaces (repositories, services)
* DTOs and validation

This is where the system’s behavior lives. If a rule breaks here, the bug is real.

### 3. Infrastructure

* Database access (Entity Framework Core)
* Repository implementations
* External services

This layer is replaceable. Swap SQL Server for something else and the core shouldn’t care.

### 4. API (Presentation)

* Controllers
* Request/response mapping
* Dependency injection

Controllers do not contain business logic. If they do, the architecture has already failed.

---

## Tech Stack

* .NET (ASP.NET Core Web API)
* Entity Framework Core
* SQL Server
* Docker & Docker Compose

Nothing exotic. Predictability beats novelty.

---

## Getting Started

### Prerequisites

* .NET SDK installed
* Docker & Docker Compose
* Git

### Clone the repository

```
git clone https://github.com/OneGangz/dotnet-clean-api.git
cd dotnet-clean-api
```

### Run with Docker

```
docker compose up -d
```

This starts:

* API container
* SQL Server container

The API will wait for SQL Server before starting.

---

## Configuration

Configuration is handled via:

* `appsettings.json`
* Environment variables (Docker)

Sensitive data should **never** be committed. If it is, assume it’s already leaked.

---

## API Design Rules

These rules are non-negotiable:

* Controllers call application services only
* No Entity Framework types outside Infrastructure
* No business logic in controllers
* No direct DB access from API layer
* Interfaces live in Application, implementations in Infrastructure

Break these rules and you no longer have Clean Architecture, just folders with confidence issues.

---

## Testing (Planned)

* Unit tests for Application layer
* Integration tests for Infrastructure
* No controller tests that mock everything into meaninglessness

---

## Versioning

This project follows simple semantic intent:

* `v0.x` – evolving structure
* `v1.0` – stable baseline architecture

---

## Why this exists

Most "Clean Architecture" examples online are either:

* Over-engineered nonsense
* Under-engineered demos
* Or both

This repo aims to be a realistic middle ground you can actually build on.

---

## License

MIT License. Use it. Break it. Improve it. Just don’t pretend you wrote it without understanding it.
