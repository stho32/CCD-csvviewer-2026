# .NET CLI Tool Architecture Guide

A layered architecture for .NET console/CLI applications with clear separation between entry point, business logic, and tests. Designed for command-line tools that perform file system operations, data processing, database operations, or automation tasks. Emphasizes testability through dependency injection, the Command/Strategy pattern, and a strict layered structure.

## Contents

1. [Architecture](./ARCHITECTURE.md) — Solution structure, patterns, project configuration, testing, CI/CD
2. [Project Scaffolding](./02-project-scaffolding.md) — Templates and initialization instructions

## Quick Reference

### Official Documentation

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/)
- [CommandLineParser GitHub](https://github.com/commandlineparser/commandline)
- [NUnit Documentation](https://docs.nunit.org/)
- [Coverlet GitHub](https://github.com/coverlet-coverage/coverlet)
- [GitHub Actions for .NET](https://learn.microsoft.com/en-us/dotnet/devops/github-actions-overview)

## Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 10.0+ | Runtime and SDK |
| CommandLineParser | 2.9.1 | CLI argument parsing |
| NUnit | 4.3.2 | Unit and integration testing |
| NUnit3TestAdapter | 5.0.0 | Test runner adapter |
| coverlet | 6.0.4 | Code coverage collection |
| GitHub Actions | -- | CI/CD pipelines |

## When to Use This Architecture

- Command-line tools for file system operations or data processing
- Database migration or administration tools
- Automation scripts requiring structured error handling
- CLI tools with dry-run / what-if mode
- Projects needing strict layered separation for testability

## When NOT to Use This Architecture

- Desktop applications with a GUI (use dotnet-avalonia-clean-architecture)
- Web applications (use ASP.NET)
- Simple one-off scripts (use python-uv-app)
- Tools that need Roslyn code analysis (use dotnet-roslyn-code-analyzer)

Last updated: April 2026
