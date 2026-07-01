# .NET CLI Tool Architecture

A layered architecture for .NET console/CLI applications with clear separation between entry point, business logic, and tests.

## Overview

This architecture is designed for command-line tools that perform file system operations, data processing, database operations, or automation tasks. It emphasizes testability through dependency injection, the Command/Strategy pattern, and a strict layered structure.

## Solution Structure

```
Source/<ProjectName>/
  <ProjectName>.sln
  <ProjectName>/                        # Entry Point (Console App)
    Program.cs
    <ProjectName>.csproj
  <ProjectName>.BL/                     # Business Logic (Class Library)
    <ProjectName>.BL.csproj
    CommandLineArguments/
      CommandLineOptions.cs             # CLI options definition (declarative)
      CommandLineArgumentsParser.cs     # Parsing logic
    Common/
      Result.cs                         # Result pattern types
      ValidationHelper.cs              # Input validation (SQL injection, etc.)
    <Domain>/                           # Domain-specific logic
      I<Domain>Repository.cs           # Repository interface
      <Domain>Repository.cs            # Repository implementation
      <Domain>Command.cs               # Command implementations
    Logging/
      ILogger.cs
      ConsoleLogger.cs
      LoggerFactory.cs
  <ProjectName>.BL.Tests/              # Unit Tests (NUnit)
    <ProjectName>.BL.Tests.csproj
    Common/
      ...Tests.cs
    <Domain>/
      ...Tests.cs
    Mocks/
      Mock<Interface>.cs               # Hand-written mocks for interfaces
  <ProjectName>.BL.IntegrationTests/   # Integration Tests (NUnit)
    <ProjectName>.BL.IntegrationTests.csproj
    <Domain>IntegrationTests.cs
```

## Key Patterns

### 1. Layered Architecture
- **Entry Point** (`<ProjectName>/`): Only wiring, validation, and console I/O. No business logic.
- **Business Logic** (`<ProjectName>.BL/`): All domain logic. No console dependencies. Fully testable.
- **Unit Tests** (`<ProjectName>.BL.Tests/`): Isolated tests with mock implementations. No external dependencies.
- **Integration Tests** (`<ProjectName>.BL.IntegrationTests/`): End-to-end tests with real databases, file systems, or services.

**Dependency Rule:** Entry Point → BL. Never reverse.

### 2. Command Pattern for Operations
Define operations as commands that can be sorted, filtered, and executed:
```csharp
public interface ICommand {
    string Name { get; }
    IOperation PrepareOperation(string basePath, IOperationFactory factory);
}
```

### 3. Factory Pattern for Execution Modes
Use factories to switch between real and simulated execution:
```csharp
public interface IOperationFactory {
    IOperation CreateOperation(...);
}
// RealOperationFactory    -> real operations (file system, database, etc.)
// WhatIfOperationFactory  -> simulation / dry-run (logging only)
```

### 4. Strategy Pattern for Data Sources
Abstract data access behind interfaces for testability:
```csharp
public interface IDataSource {
    DataCollection GetData(string path);
}
// RealDataSource       -> production implementation
// InMemoryDataSource   -> test implementation
```

For database access, use the Repository Pattern:
```csharp
public interface IDatabaseAccessor : IDisposable {
    Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
    Task<bool> ExecuteInTransactionAsync(Func<IDatabaseAccessor, Task<bool>> operation);
}

public interface I<Domain>Repository {
    // Domain-specific data access methods
}
```

### 5. Result Pattern for Error Handling
All public methods return Result objects — never throw exceptions across layer boundaries:
```csharp
public record Result(bool IsSuccess, string Message);
public record Result<T>(T? Value, bool IsSuccess, string Message);
```

### 6. Input Validation
Validate all external inputs at system boundaries (CLI arguments, user-provided names).
Use a central `ValidationHelper` for reusable validation logic:
```csharp
public static class ValidationHelper {
    public static bool IsValidIdentifier(string? name) { ... }
}
```

## Project Configuration

### Entry Point (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\<ProjectName>.BL\<ProjectName>.BL.csproj" />
  </ItemGroup>
</Project>
```

### Business Logic (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="CommandLineParser" Version="2.9.1" />
    <!-- Add domain-specific packages here, e.g.: -->
    <!-- <PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" /> -->
  </ItemGroup>
</Project>
```

### Unit Tests (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />
    <PackageReference Include="NUnit" Version="4.3.2" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\<ProjectName>.BL\<ProjectName>.BL.csproj" />
  </ItemGroup>
</Project>
```

### Integration Tests (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />
    <PackageReference Include="NUnit" Version="4.3.2" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\<ProjectName>.BL\<ProjectName>.BL.csproj" />
  </ItemGroup>
</Project>
```

## Testing Strategy

### Unit Tests
- Test business logic with hand-written mock implementations of interfaces
- Test command sorting, parsing, data transformations, validation
- Mock directory: `Mocks/` with `Mock<InterfaceName>.cs` per interface
- Target: 100% coverage on public methods of domain logic

### Integration Tests
- Separate project (`<ProjectName>.BL.IntegrationTests/`)
- Use real implementations with temporary directories or real databases
- Create test fixtures in SetUp, clean up in TearDown
- Test full pipeline: data source -> commands -> operations -> result verification
- For database tests: use dedicated test database, not production

### E2E Tests
- Test CLI argument parsing with various input combinations
- Test WhatIf mode produces no side effects
- Test exit codes for success and failure scenarios

### Coverage
- Use `coverlet.collector` in both test projects
- Measure with: `dotnet test <solution> --collect:"XPlat Code Coverage"` (immer ueber Solution, nicht einzelne Projekte — Integration-Tests decken Code ab den Unit-Test-Mocks nicht erreichen)
- Bei mehreren Coverage-Reports: `reportgenerator` zum Mergen nutzen
- Target: >80% line coverage for BL project

## CI/CD

### GitHub Actions Workflow
```yaml
name: Build and Test

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
      - run: dotnet restore
      - run: dotnet build --configuration Release --no-restore
      - run: dotnet test --configuration Release --no-restore --collect:"XPlat Code Coverage"
      - run: dotnet publish -c Release -o output -r win-x64 --self-contained true
      - uses: actions/upload-artifact@v4
        with:
          name: release
          path: output/
```

## Static Application Security Testing (SAST)

### CodeQL — Automatische Sicherheitsanalyse

CodeQL analysiert C#-Code auf Sicherheitsschwachstellen (OWASP Top 10: Injection, XSS, Path Traversal, etc.) und laeuft als eigener GitHub Workflow.

**GitHub Workflow** (`.github/workflows/codeql.yml`):
```yaml
name: "CodeQL"

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  schedule:
    - cron: '0 6 * * 1'

jobs:
  analyze:
    name: Analyze C#
    runs-on: ubuntu-latest
    permissions:
      security-events: write
      contents: read
      actions: read
    steps:
      - uses: actions/checkout@v4
      - name: Initialize CodeQL
        uses: github/codeql-action/init@v3
        with:
          languages: csharp
          queries: security-extended
      - uses: github/codeql-action/autobuild@v3
      - name: Perform CodeQL Analysis
        uses: github/codeql-action/analyze@v3
```

**Stufenplan:**
1. **Stufe 1**: `security-extended` Suite aktivieren — sofortiger Nutzen ohne Regelaufwand
2. **Stufe 2**: Nach ein paar Wochen Alerts im GitHub Security Tab auswerten
3. **Stufe 3**: Custom Queries fuer projektspezifische Muster ergaenzen (optional)

**Hinweis:** Fuer private Repos ist GitHub Advanced Security erforderlich.

## Dependabot

`.github/dependabot.yml` fuer automatische NuGet- und GitHub-Actions-Updates:

```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
```

## Docker Support (optional)

For tools that require external services (databases, message queues, etc.), provide a `docker-compose.yml`:
```yaml
services:
  <service>:
    image: <image>
    ports:
      - "<port>:<port>"
    healthcheck:
      test: <check command>
      interval: 10s
      retries: 5
      start_period: 30s
```

Include init scripts in `docker/` for automatic setup of test databases or seed data.

## Conventions

- **Nullable**: Always enabled (`<Nullable>enable</Nullable>`)
- **Zero warnings**: `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in all projects
- **Naming**: PascalCase for types/methods, camelCase for locals, _camelCase for private fields
- **Exit codes**: `Main()` returns `int` — 0 for success, non-zero for failure
- **Validation**: Validate external inputs at system boundaries. Use `ValidationHelper` for reusable checks.
- **Error handling**: Try/catch in operations, return Result objects instead of throwing
- **Logging**: `ILogger` interface with `LoggerFactory.Get()`. Console implementation for CLI. Never log sensitive data (connection strings, credentials).
- **File-scoped namespaces**: Use `namespace X;` (not block form)
- **Records**: Prefer records for immutable data types (Result, CommandInfo, Models)
