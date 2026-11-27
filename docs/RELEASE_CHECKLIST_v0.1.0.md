# Current active-UFW acceptance checklist — 2026-11-27
UFW is ACTIVE in the existing isolated Docker container;83 final active-lab assertions PASS. Full evidence, initial harness failures and exact scope: TEST_REPORT_v0.1.0.md and active-*-results.json.

- [x] Existing capabilities/network preserved; no WSL/VM/host firewall changes.
- [x] Active IPv4 and IPv6 kernel rule inventory matches Admin/Viewer API (8 fixture rules; final2 HTTPS-only rules).
- [x] Actual IPv4 ingress allow/deny/restoration tested; temporary rules/users cleaned.
- [x] Current Ubuntu PAM lock/unlock, expired account/password-change-required, root/system/empty rejection tested.
- [x] API/CSRF/roles/session/error/security regressions repeated under active UFW.
- [x] UFW reload and real API process restart preserve expected inventory/key/cookie/CSRF behavior.
- [x] Recent audit reset and private console-file retention verified; no journal durability claim.
- [ ] BLOCKED: external IPv6 traffic (bridgeIPv4-only, eth0IPv6 disabled).
- [ ] BLOCKED: systemd runtime, real reboot/boot persistence and durable journald (no approved VM).
- [ ] BLOCKED: fresh visual browser acceptance for active UFW (CUA initialization failure); prior SVG UI tests historical.
- [ ] Independent security/PR approval pending.
- [ ] New verified candidate archive incorporating SVG UI required; old RC1 archive immutable.
- [ ] Stable merge/tag/publication not performed.
The enable command's startup flag does not prove Docker startup or real-boot execution. Read-only API has no firewall mutations.

---
## Earlier evidence (historical; superseded where the active-UFW scope above differs)
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

## Post-RC1 transparent SVG and header update — 2026-11-27
At the owner's request, replaced header/login PNG references with transparent path-only SVG derivatives traced from the original approved artwork. Flat-color tracing simplifies raster texture; it is not an original artist-supplied vector. A light ink variant preserves contrast on dark surfaces. Original PNG/ICO hashes remain unchanged. Removed the opaque CSS backing and the slogan beneath the logo; redesigned header brand/context, read-only indicator, user identity/role and sign-out alignment.

Targeted validation PASS: Production Angular build208.81kB, original/derivative source/build integrity, SVG path-only structure, actual verified HTTPS image/svg+xml delivery, desktop sign-in and authenticated Viewer header, mobile390px public/authenticated layout, narrow320px authenticated layout, and sign-out. No horizontal overflow in checked layouts (body390/375/305px for viewport390/390/320). Temporary viewer removed; viewport reset. Backend, session and PAM behavior were not changed or rerun wholesale. Prior87-check and13-scenario acceptance applies to the earlier packaged RC1, not a fresh full acceptance of this UI follow-up.

Live Docker wwwroot updated only; earlier UI retained at /opt/wallforge/api/wwwroot.before-svg-header. Existing RC1 tarball/SHA remains immutable and contains the prior PNG header. It must be rebuilt as a later candidate before these new UI changes can be distributed in a release archive. Independent security/VM/stable-release gates remain unchanged; no main merge/tag/Release.
