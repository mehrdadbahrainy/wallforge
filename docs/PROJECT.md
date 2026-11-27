# WallForge — Project brief
Updated: 2026-11-27 | Status: v0.1.0 validation completed within available local scope; **NOT READY for release**.

## Current milestone
Secure read-only Ubuntu foundation: Angular/TypeScript, unprivileged ASP.NET Core API, PAM authentication, wallforge-admin/viewer groups, UFW and local-account inventory, bounded admin audit. Firewall/account writes, Safe Apply and rollback are future milestones, outside v0.1.0.

## Verified work
Development branch feature/v0.1.0-foundation. Application fix f30f48559212117071691012ff06aafee8c73ef5, CI fixture follow-up 8a454a41d1dea1def9a441a43912022002f7206b. 78 local checks passed (73 automated +5 clean-container installation acceptance checks) plus13 documented Chrome scenarios; builds/typecheck passed. All four CI jobs passed for8a454a4, and its checksummed artifact installed successfully in a fresh Docker container. Follow CI_STATUS_v0.1.0.md for final documentation-head CI. Existing draft PR: https://github.com/mehrdadbahrainy/wallforge/pull/2 (review approval pending).

## Remaining release gates / next actions
1. Owner supplies approved Forge Shield Emblem from earlier ChatGPT conversation; no redesign/substitute authorized. Integrate logo/favicon and retest.
2. Disposable real Ubuntu24.04 VM: clean install, startup/reboot, PAM policies, systemd/journald and key continuity; Docker is insufficient.
3. Active IPv4/IPv6 UFW visibility, additional browsers and timed idle-expiration test.
4. Independent PAM/sudoers/hardening and PR review; reconcile final CI and release checklist.
5. Obtain explicit approval before merge, final tag or GitHub Release. Nothing published.

## Validation documents
TEST_REPORT_v0.1.0.md — environment, commit IDs, scenario matrix, evidence, limitations.
ISSUES_v0.1.0.md — reproductions, causes, fixes, status.
RELEASE_CHECKLIST_v0.1.0.md — remaining acceptance/publishing gates.
RELEASE_NOTES_v0.1.0.md — unpublished candidate notes.
CI_STATUS_v0.1.0.md — final observed CI and review state.

## Historical status
2026-11-24 report: 5/15 acceptance criteria complete; initial builds/integration reported successful. Previous logo selection is confirmed historically, but its asset is absent. Earlier reports are not substituted for live evidence.

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
