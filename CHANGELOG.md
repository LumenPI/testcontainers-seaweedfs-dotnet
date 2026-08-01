# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial `SeaweedFsBuilder` / `SeaweedFsContainer` implementation.
- S3-compatible endpoint, credential, and startup-bucket helpers.
- AWS SDK for .NET integration tests, verified against Podman.
- Community Module Registry submission template.
- Documented and verified compatibility with the `chrislusf/seaweedfs-enterprise`
  image (no separate module needed — the container-level interface is identical).
