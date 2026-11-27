# WallForge v0.1.0 — CI and review status

Observed 2026-11-27. Source branch `feature/v0.1.0-foundation`.

- Application fixes: `f30f48559212117071691012ff06aafee8c73ef5`.
- Verified follow-up revision: `8a454a41d1dea1def9a441a43912022002f7206b`.
- [Successful workflow run 60](https://github.com/mehrdadbahrainy/wallforge/actions/runs/45992322959): all four jobs passed — API warning-as-error build/smoke/HTTP denial; Angular npm-ci/state regressions/typecheck/build; Linux PAM/UFW/authenticated HTTPS/cookie-expiry/unavailable-UFW; self-contained Linux preview packaging and artifact upload.
- Artifact ID `15194916232`, `wallforge-local-preview-linux-x64`; ZIP digest `sha256:e1a7e69ddb06db70795e1d7f060e23dd095c4ae6e9fda3b2bc3dcfbabdcfcd94`. This is a short-lived CI preview artifact, not a GitHub Release.
- [Earlier run 58](https://github.com/mehrdadbahrainy/wallforge/actions/runs/45992034915) failed only the new expiration probe's public-certificate read permission; follow-up corrected the public CRT to0644. Private key/PFX restrictions were retained.
- [Existing PR #2](https://github.com/mehrdadbahrainy/wallforge/pull/2) remains draft; no submitted reviews observed. Ready-for-release/review-approved status is not asserted.

Documentation/evidence may be committed after this tested revision; follow the PR's live checks for that final documentation head. Application code remains the verified code above. Final observed head and checks are also recorded in the acceptance summary and SharedSpace copy.

No merge, final version tag, or release publication performed. **Release readiness: NOT READY** until logo, VM/host checks and independent review gates close.

Clean-install verification: the artifact ZIP and tar SHA256 matched; packaged installer and HTTPS/session/unavailable-UFW checks passed in a fresh Ubuntu container. See clean-container-results.json and sanitized install log.
