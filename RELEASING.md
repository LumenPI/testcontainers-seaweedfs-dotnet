# Releasing

## One-time repository setup

1. Create a NuGet.org account (or use an existing one under the `lumenpi` org/user).
2. Generate a NuGet.org API key scoped to the `SeaweedFs.Testcontainers*` package pattern.
3. Add it as the `NUGET_API_KEY` secret on the GitHub repository.
4. Add a publish job (`dotnet nuget push`) to `.github/workflows/build.yml`, gated on
   pushed tags (e.g. `v*`), once ready to automate releases.

## Release checklist

1. Update `Version`/`PackageVersion` (or rely on `MinVer`/`Nerdbank.GitVersioning` if
   adopted later) and remove any pre-release suffix.
2. Run `dotnet build --configuration Release` and `dotnet test --configuration Release`.
3. `dotnet pack --configuration Release` and inspect the generated `.nupkg`.
4. `dotnet nuget push` to NuGet.org, or push a Git tag if CI is wired to publish on tag.
5. Update the version in `README.md` and the registry entry.
6. Add the release notes to `CHANGELOG.md`.
