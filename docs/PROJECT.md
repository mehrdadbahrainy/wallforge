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
