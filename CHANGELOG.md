# Changelog

All notable changes to WallForge will be documented here.

This project follows [Semantic Versioning](https://semver.org/) and a versioned [roadmap](docs/ROADMAP.md). Use the categories **Added**, **Changed**, **Fixed**, **Security**, **Breaking**, and **Known issues** where relevant.

> **No version of WallForge has been released yet.** Entries below are unreleased development work only, not shipped capabilities.

## [Unreleased]

### Added
- Angular 22 initial read-only management UI with login, firewall, accounts, and audit views (development branch).
- ASP.NET Core .NET 10 API with PAM authentication, Linux group-based roles, CSRF-protected sessions, login rate limiting, and read-only UFW/users endpoints.
- Fixed-command restricted sudoers policy, PAM service configuration, systemd development unit, and documented security boundaries.
- CI builds Angular and API; runs parsing/authorization smoke tests, Ubuntu PAM/UFW integration and authenticated HTTPS API session tests.
- English and Persian developer installation guides.
- Initial project vision, security principles, and preliminary architecture.
- Version-by-version roadmap through v1.2.0 with acceptance criteria and a Definition of Done.
- Standardized release note requirements.

### Changed
- None.

### Fixed
- None.

### Security
- Reworked PAM authentication to use a narrowly scoped, root-owned native verifier (after direct non-root PAM authentication failed on Ubuntu CI). Implemented role revocation checks, HTTPS-only session cookies and antiforgery protection in source. **PAM/UFW and HTTPS session behavior passed on a GitHub-hosted Ubuntu 24.04 runner.** Independent security review, real browser E2E and manual installation/upgrade verification are still pending. Safe Apply/host-side rollback remains planned for v0.2.0.

### Breaking
- None.

### Known issues
- Automated builds, Ubuntu PAM/UFW and authenticated HTTPS API integration tests pass in CI. No tagged installer or verified application release is available. Browser UI and manual deployment acceptance tests are pending.
- No production security audit or system-level recovery test has been performed.

---

## Notes for maintainers

On each actual release:

1. Create a heading `## [X.Y.Z] - YYYY-MM-DD` and move only **implemented** items out of Unreleased.
2. Include **Added / Changed / Fixed / Security / Breaking / Known issues** sections, omitting empty sections as appropriate.
3. Add migration and operational recovery instructions when needed.
4. Link relevant PRs/issues and the release tag.
5. Do not mark a milestone complete unless its defined acceptance tests pass.
