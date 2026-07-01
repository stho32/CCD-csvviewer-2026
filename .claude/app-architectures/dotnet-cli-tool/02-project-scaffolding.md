# 02 — Project Scaffolding

Instructions for initializing a new .NET CLI Tool project from scratch. Used by `/erstelle-app dotnet-cli-tool`.

## Prerequisites

| Tool | Version | Install |
|---|---|---|
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Git | 2.x | https://git-scm.com/ |

Verify: `dotnet --version` should show `10.0.x` or higher.

## Step 1: Create Project Root Structure

```
<ProjectName>/
├── Anforderungen/
├── Source/<ProjectName>/
├── CLAUDE.md
└── README.md
```

```bash
mkdir -p <ProjectName>/Anforderungen
mkdir -p <ProjectName>/Source/<ProjectName>
```

## Step 2: Create Solution and Projects

```bash
cd <ProjectName>/Source/<ProjectName>

# Create solution
dotnet new sln -n <ProjectName>

# Create entry point (Console App)
dotnet new console -n <ProjectName>
dotnet sln <ProjectName>.sln add <ProjectName>/<ProjectName>.csproj

# Create Business Logic layer
dotnet new classlib -n <ProjectName>.BL
dotnet sln <ProjectName>.sln add <ProjectName>.BL/<ProjectName>.BL.csproj

# Create unit test project
dotnet new nunit -n <ProjectName>.BL.Tests
dotnet sln <ProjectName>.sln add <ProjectName>.BL.Tests/<ProjectName>.BL.Tests.csproj

# Create integration test project
dotnet new nunit -n <ProjectName>.BL.IntegrationTests
dotnet sln <ProjectName>.sln add <ProjectName>.BL.IntegrationTests/<ProjectName>.BL.IntegrationTests.csproj
```

## Step 3: Add Project References

```bash
cd <ProjectName>/Source/<ProjectName>

# Entry Point depends on BL
dotnet add <ProjectName>/<ProjectName>.csproj reference <ProjectName>.BL/<ProjectName>.BL.csproj

# Unit Tests depend on BL
dotnet add <ProjectName>.BL.Tests/<ProjectName>.BL.Tests.csproj reference <ProjectName>.BL/<ProjectName>.BL.csproj

# Integration Tests depend on BL
dotnet add <ProjectName>.BL.IntegrationTests/<ProjectName>.BL.IntegrationTests.csproj reference <ProjectName>.BL/<ProjectName>.BL.csproj
```

## Step 4: Configure .csproj Files

Replace the auto-generated .csproj contents with the following templates.

### `<ProjectName>/<ProjectName>.csproj` (Entry Point)

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

### `<ProjectName>.BL/<ProjectName>.BL.csproj` (Business Logic)

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
  </ItemGroup>
</Project>
```

### `<ProjectName>.BL.Tests/<ProjectName>.BL.Tests.csproj` (Unit Tests)

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

### `<ProjectName>.BL.IntegrationTests/<ProjectName>.BL.IntegrationTests.csproj` (Integration Tests)

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

## Step 5: Create BL Directory Structure

```bash
cd <ProjectName>/Source/<ProjectName>/<ProjectName>.BL

mkdir -p CommandLineArguments
mkdir -p Common
mkdir -p Logging
```

### `Common/Result.cs`

```csharp
namespace <ProjectName>.BL.Common;

public record Result(bool IsSuccess, string Message);
public record Result<T>(T? Value, bool IsSuccess, string Message);
```

### `CommandLineArguments/CommandLineOptions.cs`

```csharp
using CommandLine;

namespace <ProjectName>.BL.CommandLineArguments;

public class CommandLineOptions
{
    [Option('v', "verbose", Required = false, HelpText = "Enable verbose output.")]
    public bool Verbose { get; set; }
}
```

### `CommandLineArguments/CommandLineArgumentsParser.cs`

```csharp
using CommandLine;

namespace <ProjectName>.BL.CommandLineArguments;

public static class CommandLineArgumentsParser
{
    public static CommandLineOptions? Parse(string[] args)
    {
        CommandLineOptions? result = null;
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options => result = options);
        return result;
    }
}
```

### `Logging/ILogger.cs`

```csharp
namespace <ProjectName>.BL.Logging;

public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message);
}
```

### `Logging/ConsoleLogger.cs`

```csharp
namespace <ProjectName>.BL.Logging;

public class ConsoleLogger : ILogger
{
    public void Info(string message) => Console.WriteLine($"[INFO] {message}");
    public void Warning(string message) => Console.WriteLine($"[WARN] {message}");
    public void Error(string message) => Console.Error.WriteLine($"[ERROR] {message}");
}
```

## Step 6: Create Entry Point

### `<ProjectName>/Program.cs`

```csharp
using <ProjectName>.BL.CommandLineArguments;
using <ProjectName>.BL.Logging;

namespace <ProjectName>;

public class Program
{
    public static int Main(string[] args)
    {
        var options = CommandLineArgumentsParser.Parse(args);
        if (options is null)
            return 1;

        ILogger logger = new ConsoleLogger();
        logger.Info("<ProjectName> started.");

        // Application logic here

        logger.Info("<ProjectName> completed.");
        return 0;
    }
}
```

## Step 7: Create .gitignore

Place at `<ProjectName>/.gitignore`:

```gitignore
# Build results
[Dd]ebug/
[Rr]elease/
x64/
x86/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates

# Rider
.idea/

# VS Code
.vscode/

# NuGet
**/[Pp]ackages/*
!**/[Pp]ackages/build/
*.nupkg
**/[Pp]ackages/*.lock.json

# Build
*.dll
*.exe
*.pdb
*.cache
project.lock.json

# Publish output
publish/
output/

# OS
.DS_Store
Thumbs.db
desktop.ini

# Test results
TestResults/
*.trx
coverage/
```

## Step 8: Create CLAUDE.md Template

Place at `<ProjectName>/CLAUDE.md`:

```markdown
# CLAUDE.md

## Projekt

<ProjectName> — <short description>

## Architektur

Layered CLI-Tool-Architektur: Entry Point -> BL (Business Logic)

- **<ProjectName>** — Einstiegspunkt, CLI-Argument-Parsing, Konsolenausgabe
- **<ProjectName>.BL** — Geschaeftslogik, Commands, Validierung, Logging-Interfaces

## Befehle

- Build: `dotnet build Source/<ProjectName>/<ProjectName>.sln`
- Test: `dotnet test Source/<ProjectName>/<ProjectName>.sln`
- Run: `dotnet run --project Source/<ProjectName>/<ProjectName>/<ProjectName>.csproj -- [args]`

## Regeln

- Entry Point enthaelt KEINE Geschaeftslogik
- Alle oeffentlichen Methoden in BL geben Result-Objekte zurueck (keine Exceptions ueber Layer-Grenzen)
- `Main()` gibt int zurueck: 0 = Erfolg, non-zero = Fehler
- `<Nullable>enable</Nullable>` und `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in allen Projekten
- Hand-written Mocks statt Mocking-Frameworks
- File-scoped Namespaces (`namespace X;`)
```

## Step 9: Delete Auto-Generated Placeholder Files

```bash
rm <ProjectName>/Source/<ProjectName>/<ProjectName>.BL/Class1.cs
```

## Step 10: Verification

```bash
cd <ProjectName>/Source/<ProjectName>

# Restore, build, and test
dotnet restore <ProjectName>.sln
dotnet build <ProjectName>.sln
dotnet test <ProjectName>.sln

# Verify project structure
ls
# Expected: <ProjectName>.sln, <ProjectName>/, <ProjectName>.BL/,
#           <ProjectName>.BL.Tests/, <ProjectName>.BL.IntegrationTests/
```

All three commands should succeed with zero errors and zero warnings.
