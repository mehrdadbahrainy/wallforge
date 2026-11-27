# WallForge v0.1.0-rc.1 — release checklist
Updated2026-11-27. **PRE-RELEASE PREPARED / UNPUBLISHED / NOT STABLE.**
Package source `72950ccbcd520b75085aadfcbadd3d9ba19af92e`; exact artifact/hash in test report and SHA256SUMS.txt.

- [x] Owner-approved logo ZIP integrated, exact asset hashes and served paths checked.
- [x] Production Angular and self-contained Linux x64 backend/native PAM helper built.
- [x] Full package installed in existing isolated Ubuntu Docker container; account credentials preserved.
- [x] 87 automated/package checks and13 current Chrome UI scenarios passed; initial readiness harness failure corrected and rerun.
- [x] PAM, group roles, CSRF, sessions, API/error cases, read-only UFW, audit and privileged timeout regressions repeated.
- [x] Prior real20-minute idle evidence retained as historical; RC protected-ticket/revocation/recovery checks passed.
- [x] Tarball, SHA256, Persian/English install guide, Release Notes and per-file manifest prepared.
- [x] Package-source CI all4 jobs passed; final evidence-head CI tracked separately.
- [ ] BLOCKED: approved Ubuntu VM clean install, systemd service restart, reboot/key continuity, durable journal, full PAM policies, active IPv4/IPv6 UFW acceptance.
- [ ] Pending independent security review and PR approval.
- [ ] Current RC additional-browser acceptance; Firefox/Safari unavailable, earlier Edge evidence historical.
- [ ] Stable release approval / main merge / tag / GitHub Release (not authorized, not performed).

No approved disposable VM. Docker PID1=docker-init; systemd runtime, service restart, real reboot/key continuity, durable journald, full PAM policy acceptance and active IPv4/IPv6 UFW inventory are BLOCKED. No WSL/VM/host firewall changes were made.
