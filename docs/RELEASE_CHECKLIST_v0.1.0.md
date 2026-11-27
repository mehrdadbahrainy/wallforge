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
- [x] Real20-minute expiration completed; Chrome and Edge available-browser workflows passed. Firefox/Safari not available/tested.
- [ ] Independent PR review/approval; no reviews observed so far.
- [ ] Update reports and release notes after remaining gates; recheck tested commit and artifact checksum.
- [ ] Explicit owner approval before merge, final version tag and GitHub Release.

Publishing sequence after all technical gates: obtain review → owner approval to merge/tag/publish → merge agreed revision → build and verify release artifacts/checksums → create approved tag/release. None of these final actions were taken during validation.

## Final handoff verification

Final branch head: `79bd5e1c63b5bbfd49fcb6919d027f808577bc90`. All four jobs in workflow run64 completed successfully: https://github.com/mehrdadbahrainy/wallforge/actions/runs/45993508927. Git working tree clean. PR#2 remains draft and unmerged; independent review pending. These final metadata updates are synchronized to SharedSpace and local handoff files; the Git evidence snapshot remains at the final head above to avoid changing the already-verified revision. No tag or Release created.

## Security follow-up acceptance

- [x] Reproduced and fixed the privileged UFW child leak; hang/cancellation/exact-argument regressions pass.
- [x] PAM buffer-cleanup review and UTF8 rejection regression; effective trust-file write restrictions checked.
- [x] Audit queue/logger contract verified; UI persistence is intentionally absent.
- [x] Hardened systemd unit statically verified; explicit private umask/core limit/journal routing added.
- [x] All four jobs pass for security revision788fa1b (run66).
- [x] Edge functional compatibility at158.0.4370.45: six local scenarios pass after owner opened the self-signed preview. Trusted TLS not asserted.
- [x] Real20-minute Chrome idle expiration, fresh login without reload and logout observed; timestamps in test report.
- [ ] BLOCKED: owner confirms no disposable VM; no existing WSL/Hyper-V instance may be used. VM clean install/service restart/reboot/journald/PAM policies/active IPv4+IPv6 UFW remain unverified.
- [ ] Approved original logo/favicon asset, independent security/PR review, and explicit final publication approval remain required.

Existing78-check baseline remains historical. No merge, tag or Release created.

Timed expiration is complete. All VM-dependent acceptance remains BLOCKED, including service restart/reboot/journal persistence and active IPv4/IPv6 rules. Independent review and original branding asset remain release gates.

The paired security API/sudoers update is live in the existing Docker preview; HTTPS health, exact UFW read, core0/0, private0600 output log and fixture cleanup passed targeted deployment checks. This does not close any VM-dependent gate.
