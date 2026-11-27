# Active UFW Docker acceptance — 2026-11-27
**UFW enabled successfully in the existing Ubuntu container. PRE-RELEASE ONLY; stable gates remain open.**

Environment: wallforge-local-lab-ubuntu-1, Ubuntu24.04.5, existing NET_ADMIN, nonprivileged separate Docker network namespace, localhost-only127.0.0.1:5443 mapping. No added capabilities, WSL/VM or host-firewall changes. API binary remains package source72950cc; live SVG/header source9946b5d. New commits below add lab tests/evidence, not application behavior.

## Final executed results
**83 active-lab assertions PASS, zero remaining FAIL in final executed cases.** This is a new scoped Docker run, separate from earlier87 package checks/13 UI scenarios. Initial harness failures and unexecuted tests are not counted as passes.

| Group | PASS | Evidence |
|---|---:|---|
| PAM/roles/API/CSRF/cookies/revocation/active UFW inventory |39|active-ufw-api-results.json|
| Malformed API/native helper input |10|active-ufw-error-results.json|
| Current PAM account/password policies |8|active-pam-policy-results.json|
| Actual published IPv4 ingress allow/deny/restoration |3|active-ufw-ingress-results.json|
| Root timeout/cancellation/argument boundary, PAM UTF8, audit and session expiry |9|active-ufw-security-results.json|
| UFW reload/API process restart/key+cookie/CSRF/audit behavior |12|active-process-restart-results.json|
| Final minimal policy Admin/Viewer exact inventory |2|active-ufw-final-results.json|

## UFW enabled and tested
ufw --force enable returned0. Actual kernel IPv4/IPv6 UFW chains and numbered rules were inspected through root CLI and the API's exact sudo timeout command. Four temporary rules per family (ALLOW5443, ALLOW18080, DENY18081, REJECT18082UDP) yielded8 active numbered entries, exactly matching Admin and Viewer API inventory. Actual published IPv4 HTTPS returned200; replacing the HTTPS allow with a verified kernel deny blocked a new connection, and restoring the original allow restored200. UFW reload retained all8 rules.

Temporary18080/18081/18082 rules were removed. **Final policy remains ACTIVE, default deny incoming/allow outgoing/deny routed, with only5443/tcp ALLOW for IPv4 and IPv6 (2 numbered rules).** Both Admin and Viewer returned that exact final CLI inventory. Test users removed. No extra Docker ports were published. Original inactive UFW configuration and kernel rules are backed up root-private in /var/lib/wallforge-lab/ufw-before-active; rollback is available, not performed because the owner requested active UFW.

The enable command's 'enabled on system startup' text is a configuration flag, not proof that Docker's sleep/docker-init starts UFW on a future container restart. No systemd or real-boot pass is claimed.

## PAM/session/audit details
Current Ubuntu PAM accepted a valid authorized account, denied a locked password, accepted after unlock, denied password-change-required and expired accounts; native protocol rejected root/system users and zero-length passwords. No common-auth/common-account policy was changed. This does not certify every possible production PAM configuration, MFA or a faillock policy that is not installed.
Actual controlled API process restart retained the service-owned keys and an existing authenticated cookie. A pre-restart authenticated CSRF token still permitted logout, after which the cookie returned401. UFW remained active and readable. Recent audit became empty as documented, while the root0600 local console file retained the earlier event. This is process/file retention, not durable journald or reboot evidence. Expired/unexpired protected tickets were also retested (401/200); historical real20-minute browser evidence remains historical.

## Initial harness failures retained
1. First ingress probe got HTTP200 because UFW skipped inserting a conflicting same-port rule; no kernel deny was installed. Corrected the fixture to remove its owned allow temporarily, assert actual kernel DROP installation, then probe and restore. Corrected run3/3 passed. Initial result remains active-ufw-ingress-initial-results.json.
2. First Windows-host restart fixture used text stdin, converting LF to CRLF for chpasswd and setting a different throwaway password than intended. API correctly returned401. Switched Docker stdin to bytes and added direct PAM/admin preflight; corrected run12/12 passed. Initial results remain active-process-restart-initial-results.json. No product authentication fix was needed.

## Still BLOCKED / unverified
- Systemd runtime, actual service restart under systemd, real reboot/boot key+firewall continuity and durable journald: no approved VM, PID1docker-init, journal runtime absent.
- **Active IPv6 rule inventory is PASS**, but external IPv6 packet enforcement remains BLOCKED: existing Docker network is IPv4-only and eth0 IPv6 disabled. The network/sysctl/host was not changed to manufacture this result.
- Current active-UFW browser visual check BLOCKED by unavailable CUA initialization (kernel-assets path error persisted after reset). No fresh visual pass claimed. Earlier SVG/header desktop/mobile UI results remain historical; API inventories were fully verified here.
- Independent security/PR approval remains pending and cannot be supplied by these agent tests.
- Earlier RC1 archive is immutable and lacks subsequent SVG UI; a later verified candidate package is needed before distribution. No merge/tag/Release was performed.

Final CI/head metadata is recorded in SharedSpace final handoff and PR #2 after this committed evidence snapshot.

---
## Earlier evidence (historical; superseded where the active-UFW scope above differs)
# WallForge v0.1.0-rc.1 — current release-readiness report
Updated 2026-11-27. **Prepared PRE-RELEASE; NOT STABLE; NOT PUBLISHED.**

## Exact build and installation
Package source commit: `72950ccbcd520b75085aadfcbadd3d9ba19af92e`. Development branch feature/v0.1.0-foundation; draft PR #2. Later evidence/test-runner commits do not change these packaged binaries.
Archive: `wallforge-v0.1.0-rc.1-linux-x64.tar.gz` (48,830,239 bytes).
SHA256: `76f2226612caee4e1b58821c073b31b81d944e69b20c9ba367dd57b56f70f682`.
Production Angular build/typecheck passed (initial206.53kB). .NET Release build with warnings-as-errors passed, 0 warnings/errors. Self-contained linux-x64 API built with existing SDK10.0.101/runtime10.0.1; native PAM helper built in existing Ubuntu24.04.5 with GCC13, warnings-as-errors and hardening flags. This is the locally tested package, not a claim of byte identity with CI artifacts or latest runtime.

Installed the entire checksummed package into existing `wallforge-local-lab-ubuntu-1`. Verified archive and per-file manifest, installer success, preservation of existing account credentials, installed API/UI bytes, and unchanged UFW configuration. API runs unprivileged at localhost-only HTTPS5443. Existing TLS and data-protection keys retained. No private keys, credentials or live data included in archive.

## Results
**87 automated/package checks PASS; 0 remaining failures in executed checks. 13 current Chrome UI scenarios PASS.** Build and CI results are separate from those counts.

| Group | PASS | Evidence |
|---|---:|---|
| .NET parsing/authorization smoke |17| Release smoke runner |
| Frontend components |7| Angular component runner |
| Installed Linux/PAM/API/roles/session/UFW |36| rc1-linux-api-results.json |
| Malformed API/native protocol/error cases |10| rc1-error-results.json |
| Focused security regressions |11| rc1-focused-security-results.json; rc1-nonmember-results.json |
| Approved source/Production brand integrity |1| tests/branding/verify-assets.py |
| Package/install/credentials/bytes/UFW preservation |5| rc1-install-results.json; rc1-install.txt |
| Current Chrome desktop/mobile UI |13| rc1-browser-results.json |

PAM valid/invalid/admin/viewer/expired/nonmember and arbitrary sudo denial passed. Valid PAM credentials without an authorized group return API401. CSRF, origin, Secure/HttpOnly/Strict cookies, no-store, role revocation, viewer403, logout401, unknownAPI404, mutation405, rate-limit429 and malformed requests passed. UFW lab was INACTIVE: empty inventory and unchanged configuration passed; active-rule parser tests and bounded hang/cancellation/altered-argument denial passed. This does not certify active IPv4/IPv6 host inventory.

An initial supplemental test invocation raced Kestrel startup and failed with ConnectionRefused. The runner now waits for verified HTTPS readiness and returns nonzero for failed result rows. The corrected rerun passed all10 cases; the initial harness failure is not counted as a product PASS.

Session coverage on this RC: actual protected-ticket expired401/unexpired200, live group revocation and browser re-login without reload passed. Historical real20-minute idle test (1218.528s) passed before RC branding/version changes; it was NOT repeated or represented as a new RC timed run. Session implementation/settings are unchanged. Earlier Edge results remain historical; current RC UI checks were Chrome. Firefox/Safari not validated.

Audit: newest250 of300 events, all300 logger emissions, and documented in-memory-store reset passed. Root-private console log retained events across controlled API process restart. This is local console capture, not durable journald/reboot evidence.

## Approved branding
Original owner-approved `SharedSpace/WallForge/wallforge-brand-v0.1.0.zip` integrated without redesign. Header/login horizontal logo, favicon and app icons are exact approved bytes. Source/Production and actual HTTPS-served asset hashes match. Desktop/mobile aspect ratio, contrast, alt text and navigation checked. Missing-logo blocker is RESOLVED. Browser automation can decorate its tab favicon; raw served HTML and ICO/PNG bytes were verified separately.

## CI and review
Package-source CI: all4 jobs PASS, run46014480577: https://github.com/mehrdadbahrainy/wallforge/actions/runs/46014480577 . Final evidence-head CI is recorded in CI_STATUS_v0.1.0.md.
Independent security review of PAM helper, sudoers and systemd hardening and PR approval remain pending. Agent testing does not count as independent review.

## BLOCKED / release gates
No approved disposable VM. Docker PID1=docker-init; systemd runtime, service restart, real reboot/key continuity, durable journald, full PAM policy acceptance and active IPv4/IPv6 UFW inventory are BLOCKED. No WSL/VM/host firewall changes were made.
Existing-container upgrade is PASS; clean real-host install/reboot is not certified. Systemd unit static checks do not substitute for runtime acceptance. Safe Apply/firewall mutations are outside this read-only release.
Stable release is blocked by independent review, VM-dependent acceptance and explicit owner approval. No main merge, tag or GitHub Release was performed. Rollback retains the matching prior startup/policy and security API in the same lab; see installation guide.

## Post-RC1 transparent SVG and header update — 2026-11-27
At the owner's request, replaced header/login PNG references with transparent path-only SVG derivatives traced from the original approved artwork. Flat-color tracing simplifies raster texture; it is not an original artist-supplied vector. A light ink variant preserves contrast on dark surfaces. Original PNG/ICO hashes remain unchanged. Removed the opaque CSS backing and the slogan beneath the logo; redesigned header brand/context, read-only indicator, user identity/role and sign-out alignment.

Targeted validation PASS: Production Angular build208.81kB, original/derivative source/build integrity, SVG path-only structure, actual verified HTTPS image/svg+xml delivery, desktop sign-in and authenticated Viewer header, mobile390px public/authenticated layout, narrow320px authenticated layout, and sign-out. No horizontal overflow in checked layouts (body390/375/305px for viewport390/390/320). Temporary viewer removed; viewport reset. Backend, session and PAM behavior were not changed or rerun wholesale. Prior87-check and13-scenario acceptance applies to the earlier packaged RC1, not a fresh full acceptance of this UI follow-up.

Live Docker wwwroot updated only; earlier UI retained at /opt/wallforge/api/wwwroot.before-svg-header. Existing RC1 tarball/SHA remains immutable and contains the prior PNG header. It must be rebuilt as a later candidate before these new UI changes can be distributed in a release archive. Independent security/VM/stable-release gates remain unchanged; no main merge/tag/Release.
