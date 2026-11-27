# Current active-UFW issues — 2026-11-27
UFW active inventory gate is now PASS within Docker for both IPv4/IPv6;83 final active-lab assertions passed. No new reproducible application failure remains in executed cases. See TEST_REPORT_v0.1.0.md.

## Resolved test-fixture issues
- UFW skipped inserting a conflicting same-port rule, so the initial ingress probe had no DROP installed. Fixture now temporarily removes its owned allow, asserts kernel DROP, tests and restores. Corrected3/3 passed; initial FAIL retained.
- Windows text stdin translated LF to CRLF for chpasswd, producing unintended fixture credentials. Binary stdin plus direct PAM preflight fixed setup; restart suite12/12 passed. API401 was correct for the mismatched password; no auth bypass/change made.

## Remaining gates
- External IPv6 packet enforcement BLOCKED by existing IPv4-only bridge/disabledeth0 IPv6; actual active IPv6 inventory PASS.
- Real systemd/reboot/journald/boot persistence BLOCKED; process restart/file capture are narrower PASS results.
- Fresh active-UFW browser visual check BLOCKED by persistent CUA initialization failure after reset.
- Independent security/PR approval pending. No stable release, merge or tag.
- Existing immutable RC1 archive lacks later SVG UI; requires a later verified candidate package.

---
## Earlier evidence (historical; superseded where the active-UFW scope above differs)
# WallForge v0.1.0-rc.1 — issues
Updated2026-11-27. Current source/package `72950ccbcd520b75085aadfcbadd3d9ba19af92e`. Prepared pre-release only.

## Resolved
- Approved logo missing: original owner ZIP supplied and integrated byte-for-byte; production/browser validation passed.
- Installer prompted to reset an existing wfadmin password: now preserves existing credentials; repeated full-package install verified unchanged shadow entry in memory (never exported).
- Test runner could race API startup and return success despite FAIL rows: verified HTTPS readiness polling and nonzero exit on failure added; supplemental10/10 passed after correction.
- Previously fixed UFW privileged timeout/argument boundary, PAM UTF8 cleanup, CSRF/logout recovery and username/API boundary regressions passed again on installed RC.

## Open release gates
- BLOCKED: No approved disposable VM. Docker PID1=docker-init; systemd runtime, service restart, real reboot/key continuity, durable journald, full PAM policy acceptance and active IPv4/IPv6 UFW inventory are BLOCKED. No WSL/VM/host firewall changes were made.
- PENDING: independent security review and PR approval. No independent reviewer approval claimed.
- PENDING: current RC additional browsers; prior Edge evidence is historical. Current Chrome desktop/mobile13/13 passed.
- Stable merge/tag/publishing requires explicit approval. No remaining reproducible failure in executed RC checks; this does not close unexecuted gates.
