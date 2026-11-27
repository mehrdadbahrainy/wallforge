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

## Final handoff verification

Final branch head: `79bd5e1c63b5bbfd49fcb6919d027f808577bc90`. All four jobs in workflow run64 completed successfully: https://github.com/mehrdadbahrainy/wallforge/actions/runs/45993508927. Git working tree clean. PR#2 remains draft and unmerged; independent review pending. These final metadata updates are synchronized to SharedSpace and local handoff files; the Git evidence snapshot remains at the final head above to avoid changing the already-verified revision. No tag or Release created.

## Latest stabilization follow-up

Security revision788fa1bc439bb18076af73ef9d1f95f6d056dd8a passes all four CI jobs in run66. Fixed unbounded privileged UFW subprocess lifetime, moved rejected UTF8 password buffers into guaranteed cleanup, and hardened private-file/core/journal service defaults. Eleven incremental focused security assertions passed; six Edge scenarios passed after the owner opened the local self-signed preview. The real20-minute Chrome test is underway; final observation will be appended. The owner confirmed all VM-dependent gates are BLOCKED and existing WSL/Hyper-V environments must remain untouched. No branding substitute, merge, tag or publication authorized. NOT READY remains.

Timed20-minute Chrome idle expiration, subsequent relogin without reload and logout are now PASS. Available browser coverage: Chrome158.0.8127.37 and Edge158.0.4370.45; other browser platforms untested. Eleven incremental security checks passed; original78-check evidence retained historically. Owner-confirmed VM blocks, logo and independent review still prevent release.

The paired security API/sudoers update is live in the existing Docker preview; HTTPS health, exact UFW read, core0/0, private0600 output log and fixture cleanup passed targeted deployment checks. This does not close any VM-dependent gate.

## Final stabilization handoff

Exact final head: `41c6751e4cd04da0322644a5bc88eaa2d59ab4fc` on feature/v0.1.0-foundation. All four jobs in run68 completed successfully: https://github.com/mehrdadbahrainy/wallforge/actions/runs/46007731035. Working tree clean. PR#2 remains draft, unmerged and without submitted review. Final head metadata is synchronized in SharedSpace/local handoff files after the Git evidence snapshot; no additional code change or release action. VM-dependent acceptance and original branding remain BLOCKED; release readiness NOT READY.
