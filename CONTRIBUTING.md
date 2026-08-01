# Contributing

Contributions are welcome.

## Development requirements

- .NET 8 SDK or newer
- A Docker-compatible container runtime (Docker Engine, Podman, etc.) reachable via the
  standard `DOCKER_HOST` / Testcontainers configuration

## Build

```bash
dotnet build
```

## Before opening a pull request

```bash
dotnet build
dotnet test
```

`dotnet test` requires a running container runtime — the integration tests will fail,
not skip, if one isn't reachable.

Please keep public APIs small, avoid adding runtime dependencies, and pin Docker images in examples and tests.
