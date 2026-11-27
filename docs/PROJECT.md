# WallForge — project brief
Updated2026-11-27. v0.1.0-rc.1 PRE-RELEASE PREPARED, unpublished; stable release blocked.

Read-only Ubuntu foundation: Angular UI, unprivileged ASP.NET Core API, native PAM verifier, admin/viewer OS groups, UFW/users inventory and bounded recent activity. Writes/Safe Apply remain future scope.

Approved original branding integrated. Production Linux x64 package built from `72950ccbcd520b75085aadfcbadd3d9ba19af92e` and installed in existing isolated Docker lab. 87 automated/package checks and13 current Chrome UI scenarios passed. Package-source CI all4 jobs passed. See TEST_REPORT_v0.1.0.md, CI_STATUS_v0.1.0.md, RELEASE_CHECKLIST_v0.1.0.md and SHA256SUMS.txt for exact evidence.

Remaining: independent security/PR review, approved real Ubuntu VM acceptance, additional current browsers and explicit stable publishing approval. No approved disposable VM. Docker PID1=docker-init; systemd runtime, service restart, real reboot/key continuity, durable journald, full PAM policy acceptance and active IPv4/IPv6 UFW inventory are BLOCKED. No WSL/VM/host firewall changes were made.
PR https://github.com/mehrdadbahrainy/wallforge/pull/2 remains draft; no main merge/tag/release.

## Existing management sheet
https://docs.google.com/spreadsheets/d/1_dZtqxKAew9Jp87qDNp6bEAyCWZuDMVp6X0W7aCjt_w/edit
Existing tracker retained in its original location; this document links to it instead of duplicating it.

## Collaboration rules
Git is source of truth for code; SharedSpace/WallForge contains coordinated status and test evidence. Never store passwords, tokens, private keys or session cookies here. Mark each result with date, source revision, actual execution and open limitations. Do not mark release complete without evidence/review/owner approval.
