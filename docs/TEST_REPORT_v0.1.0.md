# WallForge v0.1.0 — Local validation report

Date: 2026-11-27 (Asia/Tehran). Author: Codex, acting on the owner's validation request.
Release readiness: **NOT READY**. No release, final tag or merge authorized/performed.

## Latest security stabilization — 2026-11-27

Application security revision: `788fa1bc439bb18076af73ef9d1f95f6d056dd8a`; branch feature/v0.1.0-foundation. All four jobs passed in https://github.com/mehrdadbahrainy/wallforge/actions/runs/45995765243. Earlier78 local checks and13 Chrome scenarios below are retained as historical evidence, not unnecessarily repeated or silently relabeled as new runs.

### Review and fixes

- Confirmed defect WF-11: the .NET UFW request timeout disposed its Process object but left the child executing. Reproduced in a network-disabled Docker fixture using harmless fake sudo/UFW files; baseline probe failed with “Timed-out UFW child survived the request”. No real firewall executable/configuration was altered in this local reproduction. Fixed with a root-owned GNU timeout in the exact sudo command: TERM at6s and KILL after1s. API and sudoers must be upgraded together; remove the previous direct-UFW grant. Ordinary hang, TERM-resistant hang, client cancellation, and altered arguments are covered. CI repeats the privilege-boundary regression on its disposable Ubuntu runner with a temporary harmless UFW executable and restores the original afterward.
- WF-12: a password above1024 UTF8 bytes could return after allocation before entering the cleanup finally block. Moved the size guard inside try/finally so rejected byte buffers are zeroed. UTF8-boundary rejection passed; memory erasure is source-reviewed, not claimed from heap forensics.
- Hardening WF-13: systemd now sets UMask0077, LimitCORE0, explicit stdout/stderr journal routing and identifier, kernel/control-group protections, realtime restriction and locked personality. Preview startup also sets umask077 and core limit0. NoNewPrivileges remains false because the exact sudo operations require elevation. Static unit verification passed; runtime systemd/PAM compatibility is BLOCKED until an approved disposable VM exists.
- PAM helper review: root/system UID rejection, bounded lengths, NUL rejection, hidden-password-only conversation, pam_authenticate plus pam_acct_mgmt, independent10-second alarm, disabled core dump, password-buffer clearing and no shell/argv secrets remain intact. The API cannot write the deployed helper, parent directory, PAM configuration, sudoers or timeout executable. This scoped source/permission review does not replace independent security approval.
- Audit review: the newest250 events are memory-only; all300 fixture events, including evicted ones, reached ILogger. A recreated store is empty by design. Explicit journal routing does not establish persistence: host journald retention, service restart/reboot durability and real PAM policy checks remain BLOCKED. No journal host policy or real host firewall was changed.
- A focused500-response experiment showed the original exception path already returns generic errors with no-store. No middleware change was retained for that ruled-out concern.

### Incremental results and provenance

Eleven new focused security assertions passed: SEC-R01..11 in security-stabilization-results.json. Logs: security-stabilization-ci.txt. Builds and existing CI regression suites ran as configured for the changed code; previous successful local Docker/UI suites were not rerun wholesale. The isolated fixture had no network or added capabilities.

Browser versions found: Chrome158.0.8127.37 and Edge158.0.4370.45. Edge automation adapter was unavailable, so the installed app was inspected through native UI. It initially displayed NET::ERR_CERT_AUTHORITY_INVALID; the assistant did not bypass it or change trust settings. The owner subsequently opened the application manually, after which six Edge scenarios passed: rendering, wrong-password rejection/field clearing, Viewer overview/no Activity, Firewall empty state, users/groups tables, and logout. Browser password saving was declined. Edge still showed “Not secure”; compatibility results are not production TLS trust acceptance. Firefox was not installed at the checked standard location; Safari is unavailable on this Windows environment. No additional browser pass is claimed.

The real Chrome idle-expiration test began2026-11-27T11:57:17.536Z (15:27:17 Asia/Tehran), using a disposable Viewer and unchanged20-minute/non-sliding session configuration on preview revision79bd5e1. The preview is deliberately not restarted until the interval completes. The Edge login/logout used a separate browser cookie and did not renew the Chrome cookie. The timed test is now complete; exact post-idle observation and successful relogin/logout are recorded in the completion section below.

### Blocked environment checks

The owner confirmed no approved disposable Ubuntu24.04 VM is available and prohibited use/modification of existing WSL2 or Hyper-V VMs. Hyper-V enumeration was denied before that clarification. No VM or WSL instance was created, started or modified. Clean VM installation, service restart, reboot/key continuity, persistent journald, real PAM lock/account-policy acceptance and active IPv4/IPv6 UFW inventory are all BLOCKED. Docker and CI successes are not substitutes.

Approved Forge Shield Emblem/logo/favicon replacement remains BLOCKED until the original asset is supplied. No substitute was generated. Independent security/PR approval and explicit merge/tag/publication approval remain open. **Release readiness: NOT READY**.

## Historical validation — build and environment

- Branch: `feature/v0.1.0-foundation`; application code tested: `f30f48559212117071691012ff06aafee8c73ef5`; CI fixture correction: `8a454a41d1dea1def9a441a43912022002f7206b`.
- Baseline checkout: `cae4c558d83366770a385fda755397c0ff6649c7`. The pre-existing binary bundle has no embedded commit identity; its exact binary provenance is not independently proven. Its Angular main bundle matched the baseline build (`main-JGVC7EJ6.js`).
- Checkout: `D:\Projects\WallForge\source`; build outputs/caches on D:. Existing .NET SDK 10.0.101 and Node 24.15.0 reused; no SDK/Node/Docker reinstall.
- Docker Desktop Linux container `wallforge-local-lab-ubuntu-1`, Ubuntu 24.04.5 LTS, published only at `127.0.0.1:5443`; fixed API also tested on container-only loopback port 5544.
- HTTPS validated against the existing local public certificate, with DNS/IP hostname verification in the Python API suite. No Windows trust-store or firewall changes.
- Browser: existing Chrome session through connected browser automation. Desktop screenshot 1920×889; mobile override 390×844, reset afterward. Browser version and Edge/Firefox/Safari coverage not verified.
- API user `wallforge-api` UID 999; PAM binary 0755 root:root; PAM configuration 0644 root:root; sudoers 0440 root:root; key directory 0700 wallforge-api:wallforge-api. Sudo list contained only the no-argument PAM verifier and `ufw status numbered`.
- All fixture passwords were disposable and kept out of reports/Drive. Temporary `wf-val-*` and `wf-ui-*` accounts removed. Existing `wfadmin` credentials unchanged.

## Count semantics and results

**78 local assertions/acceptance checks PASS, 0 remaining FAIL** (73 original automated checks plus 5 clean-container installation checks): 17 .NET smoke + 7 component state + 36 Linux/API checks + 10 input/helper protocol + 2 protected-ticket lifetime checks + 1 absent-UFW check. These are assertions/acceptance checks, not 78 independent test suites. Build/type gates and browser scenarios are counted separately. Baseline failures are retained as defect evidence, not hidden.

Local gates: .NET warning-as-error build PASS (0 warnings/errors); Angular typecheck PASS; production build PASS (206.31 kB initial raw bundle); self-contained Linux API and harness publish PASS; integration Bash syntax PASS.

The six original component scenarios produced 2 PASS/4 FAIL against the baseline and 6 PASS/0 FAIL after fixes; the final suite adds relogin recovery and passes 7/7. Baseline API suite: 32 PASS/1 FAIL, with unknown API route returning 200. Final expanded suite: 36/36 PASS; no-store regressions included.

## Reproduction commands

```bash
dotnet build src/WallForge.Api/WallForge.Api.csproj -c Release -warnaserror
dotnet run -c Release --project tests/WallForge.SmokeTests/WallForge.SmokeTests.csproj
cd frontend
npm ci --no-audit --no-fund
npm test
npm run typecheck
npm run build
```

Linux target binaries were published with `-r linux-x64 --self-contained true`, copied into the existing container, then executed as `wallforge-api`. `tests/e2e/local-container.py` is a **disposable lab-only** root fixture coordinator: creates/removes its own accounts, runs the unprivileged harness and read-only API/UFW checks, and uses the existing public TLS certificate. It defaults to `/tmp/wallforge-validation-integration/WallForge.LinuxIntegration` and `https://localhost:5443`; set `WF_BASE=https://localhost:5544` for a second test instance. It must never be run on production or a non-disposable host.

Additional probes:

```bash
# Run as wallforge-api in the configured lab; no ticket is printed.
/tmp/wallforge-validation-integration/WallForge.LinuxIntegration --session-expiry https://localhost:5544 /var/lib/wallforge/keys /opt/wallforge/tls/localhost.crt wfadmin
# Clean, unprivileged Ubuntu container, no network, read-only binary mount:
docker run --rm --network none --user 65534:65534 -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 --mount type=bind,source=D:\Projects\WallForge\validation\linux-integration,target=/test,readonly ubuntu:24.04 /test/WallForge.LinuxIntegration --ufw-unavailable
```

The first clean-container probe failed because the minimal Ubuntu image lacked ICU. Repeating with invariant globalization for this missing-executable error-path probe passed. This does not establish that the application can be installed without its documented runtime dependencies.

## Detailed automated matrix

Every row below uses the Docker environment and fixed application code SHA above, except the Windows smoke/component rows explicitly described later. Supplemental input tests used the same fixed API and existing unchanged native helper. Evidence JSON is stored alongside this report and in the repository's `docs/evidence/v0.1.0` directory.

| Test ID | Scenario | Expected | Actual | Status | Evidence | Issue/fix |
|---|---|---|---|---|---|---|
| LIN-01 | Linux PAM/group harness: admin/read-ufw | Safe accepted/denied result described by scenario | PASS read-only UFW status: inactive; PASS Linux integration: read-ufw / admin | PASS | fixed-linux-results.json | — |
| LIN-02 | Linux PAM/group harness: viewer/valid | Safe accepted/denied result described by scenario | PASS Linux integration: valid / viewer | PASS | fixed-linux-results.json | — |
| LIN-03 | Linux PAM/group harness: admin/invalid-password | Safe accepted/denied result described by scenario | PASS Linux integration: invalid-password / admin | PASS | fixed-linux-results.json | — |
| LIN-04 | Linux PAM/group harness: none/valid | Safe accepted/denied result described by scenario | PASS Linux integration: valid / none | PASS | fixed-linux-results.json | — |
| LIN-05 | Expired PAM account denied | Safe accepted/denied result described by scenario | PASS Linux integration: account-disabled / admin | PASS | fixed-linux-results.json | — |
| SEC-01 | Arbitrary sudo command denied | Safe accepted/denied result described by scenario | exit=1 | PASS | fixed-linux-results.json | — |
| API-01 | HTTPS health with certificate validation | Safe accepted/denied result described by scenario | 200: {"status":"ok","version":"0.1.0-dev"} | PASS | fixed-linux-results.json | — |
| API-02-me | Unauthenticated /api/auth/me | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| API-02-users | Unauthenticated /api/host/users | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| API-02-firewall | Unauthenticated /api/host/firewall | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| API-02-audit | Unauthenticated /api/audit | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| SEC-02 | Login without CSRF rejected | Safe accepted/denied result described by scenario | 400 | PASS | fixed-linux-results.json | — |
| AUTH-01 | Invalid password rejected without secret details | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| AUTH-02 | Root input rejected | Safe accepted/denied result described by scenario | 400 | PASS | fixed-linux-results.json | — |
| AUTH-03 | PAM admin login | Safe accepted/denied result described by scenario | 200 | PASS | fixed-linux-results.json | — |
| SEC-03 | Secure HttpOnly Strict session cookie | Safe accepted/denied result described by scenario | Required flags present | PASS | fixed-linux-results.json | — |
| API-03-users | Admin read /api/host/users | Safe accepted/denied result described by scenario | 200; cache-control=no-store | PASS | fixed-linux-results.json | — |
| SEC-09-users | Sensitive response forbids caching | Safe accepted/denied result described by scenario | no-store | PASS | fixed-linux-results.json | WF-02 |
| API-04 | Inventory includes fixture users/groups | Safe accepted/denied result described by scenario | Fixture identities present | PASS | fixed-linux-results.json | — |
| API-03-firewall | Admin read /api/host/firewall | Safe accepted/denied result described by scenario | 200; cache-control=no-store | PASS | fixed-linux-results.json | — |
| SEC-09-firewall | Sensitive response forbids caching | Safe accepted/denied result described by scenario | no-store | PASS | fixed-linux-results.json | WF-02 |
| UFW-01 | UFW inactive status and no rules | Safe accepted/denied result described by scenario | {"available":true,"status":"inactive","rules":[],"error":null} | PASS | fixed-linux-results.json | — |
| API-03-audit | Admin read /api/audit | Safe accepted/denied result described by scenario | 200; cache-control=no-store | PASS | fixed-linux-results.json | — |
| SEC-09-audit | Sensitive response forbids caching | Safe accepted/denied result described by scenario | no-store | PASS | fixed-linux-results.json | WF-02 |
| SEC-04 | Firewall mutation endpoint denied | Safe accepted/denied result described by scenario | 405 | PASS | fixed-linux-results.json | — |
| SEC-05 | Anonymous token invalid after login | Safe accepted/denied result described by scenario | 400 | PASS | fixed-linux-results.json | — |
| SEC-06 | Cross-origin logout rejected | Safe accepted/denied result described by scenario | 400 | PASS | fixed-linux-results.json | — |
| AUTH-04 | Live group removal revokes session | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| AUTH-05 | PAM viewer login | Safe accepted/denied result described by scenario | 200 | PASS | fixed-linux-results.json | — |
| SEC-07 | Viewer audit forbidden | Safe accepted/denied result described by scenario | 403 | PASS | fixed-linux-results.json | — |
| AUTH-06 | Viewer inventory allowed | Safe accepted/denied result described by scenario | 200 | PASS | fixed-linux-results.json | — |
| AUTH-07 | Logout completes | Safe accepted/denied result described by scenario | 204 | PASS | fixed-linux-results.json | — |
| AUTH-08 | Logged-out session denied | Safe accepted/denied result described by scenario | 401 | PASS | fixed-linux-results.json | — |
| SEC-08 | Login rate limiter reaches 429 | Safe accepted/denied result described by scenario | 429,429,429,429,429,429 | PASS | fixed-linux-results.json | — |
| API-05 | Unknown API endpoint returns 404 | Safe accepted/denied result described by scenario | 404 | PASS | fixed-linux-results.json | WF-01 |
| UFW-02 | UFW configuration unchanged | Safe accepted/denied result described by scenario | Before/after /etc/ufw SHA256 equal | PASS | fixed-linux-results.json | — |
| INPUT-01 | Reject invalid or unauthorized username | Safe accepted/denied result described by scenario | 400 | PASS | supplemental-results.json | — |
| INPUT-02 | Reject invalid or unauthorized username | Safe accepted/denied result described by scenario | 400 | PASS | supplemental-results.json | WF-05 |
| INPUT-03 | Reject invalid or unauthorized username | Safe accepted/denied result described by scenario | 400 | PASS | supplemental-results.json | WF-05 |
| INPUT-04 | Reject invalid or unauthorized username | Safe accepted/denied result described by scenario | 401 | PASS | supplemental-results.json | — |
| INPUT-05 | Malformed JSON returns 400 without stack trace | Safe accepted/denied result described by scenario | 400 | PASS | supplemental-results.json | — |
| PAM-PROTO-1 | Malformed helper protocol fails closed | Safe accepted/denied result described by scenario | Denied | PASS | supplemental-results.json | — |
| PAM-PROTO-2 | Malformed helper protocol fails closed | Safe accepted/denied result described by scenario | Denied | PASS | supplemental-results.json | — |
| PAM-PROTO-3 | Malformed helper protocol fails closed | Safe accepted/denied result described by scenario | Denied | PASS | supplemental-results.json | — |
| PAM-PROTO-4 | Malformed helper protocol fails closed | Safe accepted/denied result described by scenario | Denied | PASS | supplemental-results.json | — |
| PAM-PROTO-5 | Malformed helper protocol fails closed | Safe accepted/denied result described by scenario | Denied | PASS | supplemental-results.json | — |

## Smoke and component assertions

Environment: Windows existing SDK/Node, source code SHA above. The smoke executable prints each check and totals 17. The component runner imports the real component with a mocked HttpClient; this is state regression coverage, not browser rendering coverage.

| IDs | Scenarios | Expected / actual | Status | Evidence / fix |
|---|---|---|---|---|
| SMOKE-01..17 | Root/command-like username rejection; normal user; passwd/group parsing; invalid UID; 32-byte name accepted/33-byte rejected/trailing newline rejected; admin/viewer/nonmember/root authorization; immediate group removal; active UFW parser and numbered rules; malformed UFW output | All 17 assertions succeeded | PASS | `dotnet run` output; WF-05 |
| COMP-01..03 | Logout HTTP/network errors (0, 400, 500) | Retain session and show “could not be confirmed”; observed | PASS | `npm test`; WF-03 |
| COMP-04 | Successful logout | Clear session/inventory/firewall/audit/view; observed | PASS | `npm test` |
| COMP-05 | Logout with expired server session | Clear local session; observed | PASS | `npm test` |
| COMP-06 | 401 during refresh | Clear all stale host/activity state; observed | PASS | `npm test`; WF-03 |
| COMP-07 | Relogin after session reset | Obtain a fresh anonymous CSRF token and submit login; observed | PASS | `npm test`; WF-04 |
| LIFE-01 | Unexpired protected ticket | HTTP 200; observed | PASS | Linux harness `--session-expiry` |
| LIFE-02 | Expired protected ticket | HTTP 401; observed | PASS | Linux harness `--session-expiry` |
| UFW-03 | Missing sudo/UFW on clean unprivileged Ubuntu | Safe unavailable snapshot with error; observed | PASS | Linux harness `--ufw-unavailable` |

Ticket-lifetime tests use service-owned data-protection keys to create unexpired/expired fixtures; they verify server ticket rejection without changing clocks or waiting 20 minutes. UI expiration feedback was exercised through group revocation. A complete timed 20-minute browser idle-expiration run was NOT RUN separately.

## Browser matrix

Thirteen scenarios were executed through the connected Chrome browser. Pre-fix core-view navigation was observed in the existing preview; final-build login/roles/logout, revocation/relogin and mobile-label regressions were exercised against the rebuilt preview. This is agent-driven browser E2E evidence; there is no standalone Playwright browser test suite in the repo and no other-browser result is asserted.

| ID | Scenario | Expected | Actual | Status | Evidence / issue |
|---|---|---|---|---|---|
| UI-01 | HTTPS startup and desktop layout | Usable app at localhost:5443 | Rendered login/overview without browser TLS interstitial | PASS | Live DOM and desktop screenshot |
| UI-02 | Invalid password | Generic rejection, password cleared | Observed error and cleared field | PASS | Browser DOM |
| UI-03 | Loading/submit state | Disabled Verifying… while waiting | Observed disabled button then normal error state | PASS | Browser DOM |
| UI-04 | Administrator login/overview | Admin role and host metrics | Inactive UFW, zero rules, users count, admin role | PASS | Final build DOM |
| UI-05 | Firewall navigation | Inactive/no numbered rules | Expected page and empty state | PASS | Pre-fix and final build DOM |
| UI-06 | Local users and groups | Tables reflecting container accounts | Both inventories rendered | PASS | Pre-fix preview DOM; final component table template unchanged |
| UI-07 | Activity | Admin recent events | Login/read-users/read-firewall entries, memory-only notice | PASS | Pre-fix preview DOM; final activity template unchanged |
| UI-08 | Logout | Return to login | Observed; session data cleared by regression tests | PASS | Final build DOM |
| UI-09 | Viewer login and navigation | Viewer role, no Activity button | Observed | PASS | Rebuilt preview DOM |
| UI-10 | Mobile overview and user tables | No body overflow, tables scroll internally | Document width 375 ≤ viewport 390; table scroll 552/293 and 337/293 | PASS | DOM measurement and 390×844 screenshot |
| UI-11 | Access revoked during session | Login shown with expiration/revocation alert | Observed after removing fixture group | PASS | Final build DOM |
| UI-12 | Restore role then login without reload | Fresh CSRF and successful login | Observed final-build admin overview | PASS | Final build DOM; WF-04 |
| UI-13 | Mobile accessible navigation | Descriptive names retained at 390px | Overview/Firewall/Local users/Activity names and navigation verified | PASS | Final build DOM; WF-06 |

## Explicit unverified release gates

| ID | Scenario | Expected | Actual | Status | Next action |
|---|---|---|---|---|---|
| BRAND-01 | Approved logo/header/login/favicon | Exact Forge Shield Emblem asset | Asset unavailable; owner confirmed it is only in an earlier chat; placeholders retained, no redesign | BLOCKED | Owner supplies original asset; integrate and retest |
| VM-01 | Clean Ubuntu VM installation and reboot | Documented install/start works | No disposable VM available in this task; Docker is insufficient | BLOCKED | Provision disposable Ubuntu 24.04 VM and follow both guides |
| UFW-04 | Active IPv4/IPv6 numbered rules | Correct live rule inventory | Container UFW inactive; parser checks only for active output | BLOCKED | Use preconfigured disposable VM; avoid host/production firewall changes |
| OPS-01 | systemd, journald persistence and reboot/key continuity | Restricted service starts and durable events persist | Container lacks systemd/journald validation | BLOCKED | VM install and restart/reboot audit checks |
| SEC-10 | Independent PAM/sudoers hardening review | Reviewer signoff | Source/permissions inspected locally; independent review absent | BLOCKED | Review helper, sudo boundary, PAM stack and service hardening |
| UI-14 | Unavailable Firefox/Safari coverage | Available supported browsers tested | Chrome and Edge workflows executed; Firefox/Safari unavailable and not tested | NOT RUN | Wider platform coverage not asserted |
| AUTH-09 | Real PAM lock/password change after active login | Documented revocation semantics | Group removal tested; password/account lock revocation beyond ticket expiry not verified | BLOCKED | Real VM PAM/account-policy acceptance |
| UI-15 | Real20-minute browser idle expiration | Reject expired session and permit fresh login | At1218.528s Refresh showed expiration/login; same-page fresh login and logout succeeded | PASS | Incremental real-time observation; source79bd5e1 session configuration unchanged by788fa1b |
| REVIEW-01 | Reviewed release candidate | Independent PR approval | Existing PR #2 remains draft, no submitted reviews observed | BLOCKED | Request/obtain review after open gates close |

## Safety, audit and current preview

Only read-only UFW status was invoked; `/etc/ufw` content hashes before/after were equal. No enable/disable/rule mutation was executed on the Windows host, production, or the lab. No change to Windows security/trust settings. The ephemeral missing-UFW container had no network and no added capabilities. Initial NET_ADMIN on the existing lab was pre-existing, not introduced here.

Audit UI displayed login and read events. Persistence across systemd/journald/reboot is unverified; this is intentionally not claimed as durable audit acceptance. Generic credential/error responses and native malformed-protocol rejection were observed. This is bounded functional/security testing, not a penetration test or independent audit.

The original `/opt/wallforge/api` binaries are retained. Preview startup was switched to the validated build; original script preserved as `/opt/wallforge/start.before-validation.sh`. Original `wfadmin` account remains available. Temporary UI sessions were signed out and fixtures removed. Final preview binary path: /opt/wallforge/validated-api; startup now uses this persistent directory.

## CI / PR evidence

- Baseline successful CI run: https://github.com/mehrdadbahrainy/wallforge/actions/runs/45971072546
- First fix run: https://github.com/mehrdadbahrainy/wallforge/actions/runs/45992034915 — API/Angular and PAM checks passed; expiration test fixture failed reading its public certificate because `umask 077` made it inaccessible. Fixed by making only that public certificate 0644, leaving private key/PFX restrictions intact.
- Follow-up CI verification is recorded in `CI_STATUS_v0.1.0.md` after completion.
- Existing draft PR: https://github.com/mehrdadbahrainy/wallforge/pull/2; no merge/release/tag created.

## Clean CI artifact installation — additional executed evidence

Artifact from successful revision8a454a41d1dea1def9a441a43912022002f7206b, workflow45992322959, artifact15194916232. Downloaded ZIP SHA256 matched GitHub's e1a7e69ddb06db70795e1d7f060e23dd095c4ae6e9fda3b2bc3dcfbabdcfcd94; tar SHA256 matched SHA256SUMS.txt:314fde32e5f645a3755aad559fa7d38f8fe2304af0ad7ca0588827d59d9a49ff.

A new ubuntu:24.04 container was created with no published ports and no added capabilities. The exact packaged install.sh ran as root with a generated throwaway password supplied through stdin. No SDK/Node was installed. Package dependencies were installed from Ubuntu repositories. Installer exit0, sudoers valid, API started unprivileged. HTTPS /api/health returned200 with CA+hostname verification and no-store. Valid/expired protected tickets returned200/401. Without NET_ADMIN, UFW inspection safely reported unavailable. Five checks passed (clean-container-results.json); sanitized installation output is clean-container-install.log. The clean container was stopped and removed afterward; the original lab remains running.

This establishes clean **Docker preview** installation only, not a clean VM/systemd installation. VM-01/OPS-01/UFW-04 remain unverified.

## Final handoff verification

Final branch head: `79bd5e1c63b5bbfd49fcb6919d027f808577bc90`. All four jobs in workflow run64 completed successfully: https://github.com/mehrdadbahrainy/wallforge/actions/runs/45993508927. Git working tree clean. PR#2 remains draft and unmerged; independent review pending. These final metadata updates are synchronized to SharedSpace and local handoff files; the Git evidence snapshot remains at the final head above to avoid changing the already-verified revision. No tag or Release created.

### Completed real idle-expiration observation

Login observed2026-11-27T11:57:17.536Z; first post-idle Refresh at2026-11-27T12:17:36.064Z; elapsed1218.5s (at least20 real minutes). No clock change, artificial ticket lifetime, API restart or group revocation was performed; the agent left the Chrome test page untouched during the interval. The separate Edge cookie was independent. Refresh returned the application to Secure sign in with “Your session has expired or access was revoked”; host/account/activity view state was cleared. Fresh login without page reload then succeeded, followed by logout. **Timed20-minute browser expiration and relogin: PASS.**

The idle UI detects expiry on its next server request; it does not proactively erase the screen at the exact20-minute boundary. This behavior and the transient cached view are disclosed for independent review. The test used preview revision79bd5e1; the20-minute non-sliding cookie settings and frontend are unchanged by security revision788fa1b. Protected-ticket expiry also passed on788fa1b in CI. No expired-ticket acceptance is asserted.

### Updated Docker preview deployment

After the timed test and successful logout, the locally built security revision788fa1b was deployed as a paired API/sudoers update. The preview uses `/opt/wallforge/security-api`, with the prior `/opt/wallforge/validated-api` retained. Rollback files: `/opt/wallforge/start.before-security.sh` and root-owned0440 `/opt/wallforge/sudoers.before-security`; restore the matching prior policy/startup together, with only one API running. The original pre-validation binaries/startup are also untouched.

Post-update targeted smoke checks: HTTPS health200, exactly one updated API process, core soft/hard limits0/0, correct read-only UFW status inactive through the new exact timeout command, sudoers mode0440 root:root, and private preview console log `/var/log/wallforge-preview.log` mode0600 root:root. Only127.0.0.1:5443 is published. Fixture account removed and network-disabled security fixture container removed. The console file is local preview output capture, not a claim of systemd/journald or reboot durability. No real host firewall, WSL instance or existing VM was changed.
