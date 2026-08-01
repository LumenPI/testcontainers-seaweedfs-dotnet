# SeaweedFs.Testcontainers

A community-maintained [Testcontainers for .NET](https://dotnet.testcontainers.org/)
module for running [SeaweedFS](https://github.com/seaweedfs/seaweedfs) as a
single-node, S3-compatible object store in integration tests.

> **Incubating:** The module is ready for use, but its public API may receive
> breaking changes before version `1.0.0`.

## Requirements

- .NET 8.0 or newer
- A Docker-compatible container runtime (Docker Engine, Podman, etc.)

## Installation

```bash
dotnet add package SeaweedFs.Testcontainers
```

> Not yet published to NuGet.org — see [RELEASING.md](RELEASING.md) for the
> steps to cut the first release.

## Basic usage

Always pin the SeaweedFS image tag used by the test suite.

```csharp
await using var seaweedFs = new SeaweedFsBuilder("chrislusf/seaweedfs:4.40")
    .WithBucket("test-bucket")
    .Build();

await seaweedFs.StartAsync();

var endpoint = seaweedFs.GetS3Url();
var accessKey = seaweedFs.GetAccessKey();
var secretKey = seaweedFs.GetSecretKey();
```

SeaweedFS runs in `weed mini` mode. The module exposes the S3 gateway on its
container port `8333`; Testcontainers maps it to an available host port.

## Using the SeaweedFS Enterprise image

`chrislusf/seaweedfs-enterprise` is a drop-in replacement for the open-source
image at the container level: same `mini` command, same environment variables
(`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `S3_BUCKET`), same startup
banner, same S3 gateway port. No license is required for development/testing
under 25TB, so `SeaweedFsBuilder` works unmodified — just swap the image:

```csharp
await using var seaweedFs = new SeaweedFsBuilder("chrislusf/seaweedfs-enterprise:4.40")
    .WithBucket("test-bucket")
    .Build();
```

This parity is verified in `SeaweedFsImageCompatibilityTest`, which runs the
same test body against both images.

## AWS SDK for .NET

Use path-style access because Testcontainers assigns a random local port.

```csharp
var config = new AmazonS3Config
{
    ServiceURL = seaweedFs.GetS3Url(),
    AuthenticationRegion = "us-east-1",
    ForcePathStyle = true,
};

using var client = new AmazonS3Client(seaweedFs.GetAccessKey(), seaweedFs.GetSecretKey(), config);
```

## API

| Method | Purpose |
| --- | --- |
| `WithCredentials(accessKey, secretKey)` | Configure S3 credentials |
| `WithAccessKey(accessKey)` | Override the default access key |
| `WithSecretKey(secretKey)` | Override the default secret key |
| `WithBucket(name)` | Create a bucket during startup |
| `WithBuckets(names...)` | Create several buckets during startup |
| `GetS3Endpoint()` | Return the mapped S3 endpoint as a `Uri` |
| `GetS3Url()` | Return the mapped S3 endpoint as text |
| `GetS3Port()` | Return the mapped S3 port |
| `GetAccessKey()` | Return the configured access key |
| `GetSecretKey()` | Return the configured secret key |
| `GetBuckets()` | Return the buckets configured for startup |

Default test credentials are `admin` and `secret`. Change them whenever the
credentials themselves are part of the behavior under test.

## Migrating from the official Minio module

| `Testcontainers.Minio` | `SeaweedFs.Testcontainers` |
| --- | --- |
| `MinioBuilder` | `SeaweedFsBuilder` |
| `MinioContainer` | `SeaweedFsContainer` |
| `WithUsername(...)` | `WithAccessKey(...)` |
| `WithPassword(...)` | `WithSecretKey(...)` |
| `GetConnectionString()` | `GetS3Url()` or `GetS3Endpoint()` |
| `GetAccessKey()` | `GetAccessKey()` |
| `GetSecretKey()` | `GetSecretKey()` |

Application code should continue to use the standard S3 API rather than a
MinIO-specific administration client.

## Build

```bash
dotnet build
```

Integration tests require a reachable Docker-compatible runtime (Docker Engine,
Podman, etc.):

```bash
dotnet test
```

## Community Module Registry

A ready-to-copy registry entry is under
`registry-entry/modules/seaweedfs-dotnet/index.md`. Confirm the published
NuGet version before opening a pull request against
`testcontainers/community-module-registry`.

## License

MIT
