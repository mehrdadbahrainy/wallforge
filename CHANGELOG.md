# Changelog

All notable changes to WallForge will be documented here.

This project follows [Semantic Versioning](https://semver.org/) and a versioned [roadmap](docs/ROADMAP.md). Use the categories **Added**, **Changed**, **Fixed**, **Security**, **Breaking**, and **Known issues** where relevant.

> **No version of WallForge has been released yet.** Entries below are documentation / planning work only, not shipped capabilities.

## [Unreleased]

### Added
- Initial project vision, security principles, and preliminary architecture.
- Version-by-version roadmap through v1.2.0 with acceptance criteria and a Definition of Done.
- Standardized release note requirements.

### Changed
- None.

### Fixed
- None.

### Security
- Documented planned PAM authentication, OS-group authorization, privileged-agent boundary, and host-side Safe Apply rollback. **Not yet implemented.**

### Breaking
- None.

### Known issues
- No functional application, build, or installer is available yet.
- No production security audit or system-level recovery test has been performed.

---

## Notes for maintainers

On each actual release:

1. Create a heading `## [X.Y.Z] - YYYY-MM-DD` and move only **implemented** items out of Unreleased.
2. Include **Added / Changed / Fixed / Security / Breaking / Known issues** sections, omitting empty sections as appropriate.
3. Add migration and operational recovery instructions when needed.
4. Link relevant PRs/issues and the release tag.
5. Do not mark a milestone complete unless its defined acceptance tests pass.
