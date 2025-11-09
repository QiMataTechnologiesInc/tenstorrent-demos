# .NET Demo

Tenstorrent .NET demo with code analyzers.

## Prerequisites

- .NET SDK 8.0+

## Building

```bash
dotnet restore
dotnet build
```

## Running

```bash
dotnet run --project src/TenstorrentDemo
```

## Development

### Linting

```bash
dotnet format --verify-no-changes
```

### Testing

```bash
dotnet test
```

## Project Structure

- `src/TenstorrentDemo/` - Main application
- `tests/TenstorrentDemo.Tests/` - Unit tests
- `TenstorrentDemo.sln` - Solution file
