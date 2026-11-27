# WallForge v0.1.0 — Release checklist

Updated 2026-11-27. **NOT READY**. Review candidate only; not approved for publication.

- [x] Existing environment preserved; source/builds on D:; installed tools reused.
- [x] .NET build, 17 smoke, Angular typecheck/build, 7 state regressions.
- [x] 36 Docker Linux/API checks, 10 input/native-helper checks, 2 ticket lifetime assertions, absent-UFW test.
- [x] Agent-driven Chrome browser login/admin/viewer/logout/revocation/relogin and desktop/mobile checks documented.
- [x] Verified CI artifact checksums and clean Docker preview installation/startup (5 additional checks); fresh test container removed.
- [x] Targeted defects committed and pushed on development branch; existing draft PR retained.
- [x] No host/production firewall mutations; UFW config hashes unchanged.
- [x] README, both installation guides, CHANGELOG, candidate notes and evidence updated.
- [x] Verified code CI success — all four jobs passed for 8a454a4; final documentation head tracked separately in CI_STATUS_v0.1.0.md.
- [ ] Owner provides original Forge Shield Emblem; integrate header/login/favicon and verify exact asset.
- [ ] Clean disposable Ubuntu24.04 VM install from documentation, service start/reboot and persistent key permissions.
- [ ] Live active IPv4/IPv6 UFW inventory on a preconfigured disposable VM, without changing production/Windows firewall.
- [ ] systemd/journald durable audit, failure/restart handling and PAM lock/account-policy checks.
- [ ] Independent review of PAM helper, exact sudoers boundary and service hardening.
- [ ] Timed20-minute browser expiration and additional supported browsers.
- [ ] Independent PR review/approval; no reviews observed so far.
- [ ] Update reports and release notes after remaining gates; recheck tested commit and artifact checksum.
- [ ] Explicit owner approval before merge, final version tag and GitHub Release.

Publishing sequence after all technical gates: obtain review → owner approval to merge/tag/publish → merge agreed revision → build and verify release artifacts/checksums → create approved tag/release. None of these final actions were taken during validation.
