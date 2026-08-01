# Community Module Registry entry

Copy the `modules/seaweedfs` directory into a checkout of
`testcontainers/community-module-registry`.

Before opening the registry pull request:

1. Replace `VERSION`/installation instructions with the published NuGet version.
2. Confirm the linked repository is public.
3. Confirm the package is live on NuGet.org.
4. Run the registry's `yamllint` command.

If `modules/seaweedfs` already exists in the registry (e.g. from another
language's implementation), add this `docs` entry (`id: dotnet`) to the
existing `index.md` instead of creating a duplicate module folder.
