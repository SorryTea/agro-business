# agro-business

A farming / agribusiness simulation written in C# / .NET 8. Originally a group
academic project; this fork is a hobby refactor focused on untangling the
inherited code into a stable, maintainable trunk.

![CI](https://github.com/SorryTea/agro-business/actions/workflows/ci.yml/badge.svg)

## Architecture

Layered, one project per layer (dependency direction: Core, then Data, then
Logic, then the front ends):

| Project               | Target         | Role                                                                  |
| --------------------- | -------------- | --------------------------------------------------------------------- |
| `01_agro.Core`        | net8.0         | Domain: plants, devices, economy (`Money`, transactions), `FarmState` |
| `02_agro.Data`        | net8.0         | Persistence: JSON save via `GameSaver`; EF6 context (under review)    |
| `03_agro.Logic`       | net8.0         | Simulation engine and market                                          |
| `04_agro.GUI`         | net8.0-windows | WPF desktop UI                                                        |
| `05_agro.Console`     | net8.0         | Console front end                                                     |
| `01b_agro.Core.Tests` | net8.0         | MSTest unit tests                                                     |

## Build and run

The solution lives in `agrobiznes/`.

    cd agrobiznes
    dotnet build
    dotnet test
    dotnet run --project 04_agro.GUI       # WPF GUI, Windows only
    dotnet run --project 05_agro.Console   # console front end, cross-platform

The full solution builds only on Windows because `04_agro.GUI` targets
`net8.0-windows`. CI runs on windows-latest for the same reason.

## Conventions

- **Commits:** Conventional Commits (`feat`, `fix`, `refactor`, `test`, `docs`,
  `chore`, `ci`), scoped by layer where useful, e.g. `fix(data): keep save on error`.
- **Branches:** `type/short-description`, e.g. `fix/gamesaver-keep-save-on-error`.
- **Flow:** branch, then PR, into protected `main`, with green CI. No direct
  pushes to `main`.
- **Merge:** squash for feature PRs, so `main` keeps one clean commit per change.
  Merge commits are reserved for exceptional cases such as trunk reconciliation.

## Tech

.NET 8, WPF, Entity Framework 6 (under review), System.Text.Json, MSTest, docfx.
