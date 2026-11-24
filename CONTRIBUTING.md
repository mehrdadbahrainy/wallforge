# Contributing to WallForge

Thanks for helping create a safer Linux host management experience.

1. Read [ROADMAP](docs/ROADMAP.md), [security model](docs/SECURITY.md) and the current open issue before starting.
2. Target one release at a time. Only v0.1.0 read-only work may be merged without a new security review.
3. Use an isolated Ubuntu VM for PAM/UFW integration tests; avoid real production servers.
4. Keep the web app in Angular + TypeScript and the API in ASP.NET Core.
5. Never introduce an endpoint that executes arbitrary commands, takes raw sudo arguments or exposes `/etc/shadow`.
6. Add smoke/unit tests for every change and document manual VM acceptance scenarios.
7. Update docs and `CHANGELOG.md` with actual changes. Do not mark planned features as shipped.
8. Open a PR and link it to the release issue. Wait for CI and review.

## Development checks

```bash
dotnet build src/WallForge.Api/WallForge.Api.csproj -c Release
dotnet run -c Release --project tests/WallForge.SmokeTests/WallForge.SmokeTests.csproj
cd frontend && npm install && npm run typecheck && npm run build
```

For security vulnerabilities, submit a private GitHub security advisory, not a public issue.
