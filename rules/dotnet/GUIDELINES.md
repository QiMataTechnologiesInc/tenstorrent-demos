# .NET Guidelines

This document provides guidelines for .NET projects in the Tenstorrent demo monorepo.

## Standards

- **.NET Version**: .NET 8.0+ (LTS)
- **Build System**: MSBuild / dotnet CLI
- **Target Framework**: net8.0
- **Language Version**: Latest (C# 12)

## Code Style

- **Formatter**: dotnet format
- **Analyzers**:
  - Microsoft.CodeAnalysis.NetAnalyzers
  - StyleCop.Analyzers
  - SonarAnalyzer.CSharp

## Project Structure

```
solution/
├── Solution.sln
├── src/
│   └── ProjectName/
│       ├── ProjectName.csproj
│       ├── Program.cs
│       └── Class.cs
├── tests/
│   └── ProjectName.Tests/
│       ├── ProjectName.Tests.csproj
│       └── ClassTests.cs
└── .editorconfig
```

## Code Conventions

- **Naming**:
  - Classes/Interfaces: PascalCase
  - Methods: PascalCase
  - Properties: PascalCase
  - Local Variables: camelCase
  - Private Fields: camelCase with _ prefix
  - Constants: PascalCase

## Project Configuration

Enable strict analysis in .csproj:
```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## Testing

- **Framework**: xUnit
- **Coverage**: Minimum 80% line coverage
- **Test Naming**: MethodName_Scenario_ExpectedBehavior
- **Organization**: One test class per production class

## Dependencies

Manage packages via PackageReference:
```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

## Documentation

- Use XML documentation comments for all public APIs
- Include `<summary>`, `<param>`, `<returns>`, `<exception>`
- Generate XML documentation files

## Example

```csharp
namespace ProjectName;

/// <summary>
/// Provides mathematical operations.
/// </summary>
public class Calculator
{
    /// <summary>
    /// Adds two integers.
    /// </summary>
    /// <param name="a">The first integer.</param>
    /// <param name="b">The second integer.</param>
    /// <returns>The sum of a and b.</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }
}
```

## .editorconfig

```ini
[*.cs]
# StyleCop
dotnet_diagnostic.SA1633.severity = none  # File header
dotnet_diagnostic.SA1200.severity = none  # Using placement

# Code style
dotnet_sort_system_directives_first = true
csharp_new_line_before_open_brace = all
```

## Analyzer Packages

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```
