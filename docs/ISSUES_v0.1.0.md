# WallForge v0.1.0 — Issues and resolutions

Date: 2026-11-27. Application fix commit: f30f48559212117071691012ff06aafee8c73ef5. CI fixture fix: 8a454a41d1dea1def9a441a43912022002f7206b. All code is on feature/v0.1.0-foundation / draft PR #2.

| ID | Reproduction | Root cause | Small fix | Verification / final status |
|---|---|---|---|---|
| WF-01 | GET /api/not-a-real-endpoint returned 200 HTML | API GET misses reached SPA fallback | Specific API GET catch-all returns JSON 404 | API-05 and CI E2E assertion; FIXED |
| WF-02 | Authenticated GET users/firewall/audit lacked Cache-Control | Only selected auth endpoints set no-store | Set no-store for all /api responses | SEC-09 users/firewall/audit; FIXED |
| WF-03 | Mock logout network/400/500 failure; refresh with 401 | Catch swallowed logout failure; 401 only cleared session | Preserve session/show failed logout; central clear of all sensitive view state | Baseline 4 failed component assertions; final suite 7/7; FIXED |
| WF-04 | Revoke group, refresh, restore group, try login without reload | Session reset cleared token but login returned immediately if token absent; old identity token also unsuitable | Fetch fresh CSRF when missing before login | COMP-07 and actual browser relogin without reload; FIXED |
| WF-05 | Username 32 a characters plus $, or trailing newline | Regex allowed 33 chars and $ anchor matched before final newline | Explicit maximum32 plus absolute end anchor | 3 added smoke checks and INPUT-02/03; FIXED |
| WF-06 | Set viewport390px; navigation names become symbols | CSS hides text spans without permanent accessible names | aria-label and current-page metadata | Final mobile DOM descriptive names and Firewall navigation; FIXED |
| WF-07 | Copy gcc multiline command in either install guide | Double backslash ends line incorrectly in shell | One continuation backslash | Source corrected; CI compiles same gcc flags as a single command. Full clean-guide VM install remains blocked; FIXED DOC / VM acceptance pending |
| WF-08 | CI expiration test runs as unprivileged API user | Public test certificate inherited umask077 | chmod0644 on public .crt only | Follow-up CI status in CI_STATUS_v0.1.0.md; FIXED; all four CI jobs passed in run60 |
| WF-09 | View header/login and request favicon | Approved asset absent; placeholders and no favicon link | No substitute created by owner instruction | BLOCKED RELEASE; owner must supply Forge Shield Emblem |
| WF-10 | Claim full host-level readiness from Docker | No VM/systemd/active UFW/independent signoff | Explicit gates in checklist | OPEN RELEASE GATES; no readiness claim |

Initial missing-UFW minimal-container probe failed for absent ICU; rerun with invariant globalization passed its narrowly scoped unavailable-executable test. No application dependency requirement was removed.

## Security stabilization follow-up

| ID | Reproduction/cause | Fix | Verification/status |
|---|---|---|---|
| WF-11 | Hanging UFW child survived the .NET request timeout; Process.Dispose does not terminate it | Exact root-owned GNU timeout supervisor,6s TERM/1s KILL; API and sudoers paired upgrade | SEC-R01..04 and root CI regression PASS; FIXED |
| WF-12 | Oversized UTF8 password returned before byte-buffer cleanup | Size guard moved inside try/finally | Boundary rejection PASS; cleanup source-reviewed; FIXED |
| WF-13 | Service relied on inherited core/file-creation/logging defaults | Explicit core0, umask0077, journal routing and conservative kernel protections | Static unit verification PASS; VM runtime acceptance BLOCKED |
| WF-14 | No approved disposable VM; host durability and active rules cannot be accepted from Docker | No host mutation; preserve blocked gates | VM install/restart/reboot/journald/PAM/UFW BLOCKED by owner-confirmed availability |

Edge local functional compatibility passed after the owner manually opened the self-signed preview; trusted production TLS and other unavailable browsers are not asserted. Timed expiration outcome is appended when actually completed. Missing approved logo remains WF-09/BLOCKED.

Timed20-minute Chrome expiry, relogin without reload and logout now PASS (actual timestamps in test report). The screen clears on next request; no proactive idle-screen timer is claimed. Other unavailable browsers remain untested.
