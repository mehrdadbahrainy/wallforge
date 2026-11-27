# WallForge v0.1.0 — Candidate release notes (UNRELEASED)

This is a review candidate, not a published release. Read-only Ubuntu host console: Angular UI, unprivileged ASP.NET Core API, Linux PAM/group roles, UFW status/rules, users/groups and admin activity. Neither role can modify host state.

Candidate fixes: API404 routing, no-store sensitive responses, reliable sign-out feedback and state clearing, fresh CSRF on relogin, strict username boundaries, accessible mobile navigation, installation shell continuation. npm lockfile and CI state/session regression tests added.

Validation and known limits: see TEST_REPORT_v0.1.0.md and RELEASE_CHECKLIST_v0.1.0.md. Approved logo/favicon missing, VM installation/systemd/journald/active IPv4/IPv6 UFW and independent security/PR review remain open. Not for public production exposure. No migration from a prior release exists; no version has shipped.

Install and rollback: follow docs/INSTALL.en.md or docs/INSTALL.fa.md in the reviewed commit on a disposable VM first. Existing local preview's original binary directory and startup script are preserved; restart with the saved original script to roll back the preview. No firewall/account mutations need rollback because this version only reads host state. Temporary test identities were removed.

Do not create final v0.1.0 tag, merge main, or publish until explicit owner approval and all release gates pass.
