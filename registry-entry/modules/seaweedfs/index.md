---
title: SeaweedFS
categories:
  - cloud
docs:
  - id: dotnet
    url: https://github.com/lumenpi/testcontainers-seaweedfs-dotnet
    maintainer: community
    example: |
      ```csharp
      await using var seaweedFs = new SeaweedFsBuilder("chrislusf/seaweedfs:4.40")
          .WithBucket("test-bucket")
          .Build();
      await seaweedFs.StartAsync();
      ```
    installation: |
      ```bash
      dotnet add package SeaweedFs.Testcontainers
      ```
description: |
  SeaweedFS is a distributed storage system that provides an S3-compatible API.
  This community module starts SeaweedFS in single-node mini mode for .NET
  integration tests and exposes helpers for credentials, buckets, and the
  mapped S3 endpoint.
---
