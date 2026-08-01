# Releasing

Publishing uses [NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) —
GitHub Actions authenticates to nuget.org via short-lived OIDC tokens instead of a
stored long-lived API key.

## One-time repository setup

1. On [nuget.org/account/trustedpublishing](https://www.nuget.org/account/trustedpublishing),
   add a policy:
   - **Repository Owner**: `LumenPI`
   - **Repository**: `testcontainers-seaweedfs-dotnet`
   - **Workflow File**: `publish.yml`
   - **Environment**: `release`
2. In the GitHub repo, create an environment named `release`
   (Settings → Environments → New environment), and add an environment secret
   `NUGET_USER` set to the nuget.org profile name used above (not the email
   address). Optionally add required reviewers on the environment as an
   approval gate before publishing.
3. `.github/workflows/publish.yml` already handles pack + OIDC login + push —
   no further workflow changes needed for a normal release.

## Release checklist

1. Update `<Version>` in `src/SeaweedFs.Testcontainers/SeaweedFs.Testcontainers.csproj`
   and remove any pre-release suffix.
2. Run `dotnet build --configuration Release` and `dotnet test --configuration Release`.
3. `dotnet pack --configuration Release -o ./artifacts` and inspect the generated
   `.nupkg` locally before tagging — package metadata is permanent once published.
4. Update the version in `README.md` and the registry entry.
5. Add the release notes to `CHANGELOG.md`.
6. Commit, then tag and push: `git tag v<version> && git push origin master --tags`.
   The `Publish to NuGet` workflow runs automatically on the tag and fails the
   build if the tag doesn't match the `<Version>` in the csproj.
