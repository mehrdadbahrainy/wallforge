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
