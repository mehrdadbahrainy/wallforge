# WallForge — Product roadmap

**Status:** Proposed plan. **None of the versions below has been released.**

Our guiding rule is **security before feature count**. Each version has an explicit scope, exclusions, and exit criteria. Target versions may be adjusted before their implementation, but changes must be reflected here and in release notes.

## Version matrix

| Release | Theme | Changes / deliverables | Exit criteria |
| --- | --- | --- | --- |
| **v0.1.0** | Foundation / read-only | Angular + ASP.NET Core structure, local privileged agent boundary, Linux PAM login, `wallforge-admin`/`wallforge-viewer` authorization, UFW status & current rule inventory, local user/group inventory, read-only dashboard, audit trail groundwork, English/Persian docs, unit tests + GitHub Actions | Nonprivileged services, default-deny access, no arbitrary command endpoint, locked/expired accounts rejected, repeatable CI and manual installation |
| **v0.2.0** | Firewall management with recovery | Create/update/remove UFW rules, IPv4/IPv6 representation, input & conflict checks, plan/diff before apply, timed host-side rollback, explicit confirmation, concurrency lock, pre-change snapshot and audit records | Tested on disposable Ubuntu VMs: valid apply, unsafe apply rejection, timeout, browser disconnect, agent/API crash, restore after reboot where feasible; SSH lockout drill with out-of-band recovery |
| **v0.3.0** | Linux users and groups | Create local human users, lock/unlock accounts, controlled group membership, account details, protected-user detection, carefully constrained delete/deactivate workflows | Never allow deletion/lockout of root, service/system accounts, self/last admin, or the only reachable admin path; PAM/role changes enforced on next privileged request; state-change tests |
| **v0.4.0** | SSH keys & sudo policies | View/add/revoke `authorized_keys`, preserve Unix ownership/modes, SSH access preview, privileged group/sudo rights with validation and review | SSH and sudo changes validated before commit; recoverable changes; deny removing last approved admin authentication path; require elevated confirmation and audit |
| **v0.5.0** | Host security visibility | Docker-published-port visibility, differences between UFW policy and actually exposed sockets, filter/search audit log, security warnings, event export | Accuracy tests on Docker/Ubuntu VMs; distinguish observed exposure from guaranteed filtering; no unsupported enforcement claims |
| **v0.6.0** | Installation & operations | Reproducible Ubuntu package/installer, least-privilege systemd services, config validation, local backup/restore, upgrades/rollback instructions, health checks, localization completion | Clean installs and upgrade tests on documented Ubuntu LTS versions; backup recovery drill and signed/provenance-aware release artifacts where supported |
| **v1.0.0** | Production-oriented stable single-host | Threat-model review, auth/security hardening, review of privileged agent & rollback implementation, performance/accessibility checks, complete admin and contributor guides, supported environment matrix | CI green, no known critical/high-risk security defects, independently validated recovery path, documented operational limitations |
| **v1.1.0** | More firewall backends | Optional nftables provider, capability-based backend interface, consistent policy model, feature parity for supported actions | UFW behavior unchanged, backend isolation tests, safe apply per backend, migration docs |
| **v1.2.0** | Multi-host management | Enrolled agents, authenticated encrypted connections, per-host policy/roles, status and audit aggregation, safe per-host apply/rollback | Host isolation, credential rotation, network partition and delayed-confirmation tests, no arbitrary remote shell or implicit trust |

## Detailed milestones

### v0.1.0 — foundation and visibility
**Added**
- Angular single-page app with responsive dark/light UI: login, Overview, Firewall (read-only), Users (read-only), Activity.
- ASP.NET Core API running under an unprivileged Linux service account.
- PAM authentication using a tightly scoped privileged authentication helper, without storing Linux passwords in the WallForge database.
- Authorization: `wallforge-admin` (read/write when supported), `wallforge-viewer` (read-only); explicitly assigned OS group membership is mandatory. PAM success alone does not authorize access.
- Protected session cookies (HttpOnly, Secure, SameSite), CSRF defenses, brute-force protection, safe logout/session expiration, and fresh group/account-state verification for privileged requests.
- Separate local privileged agent boundary for narrowly defined host inspection operations.
- UFW status/rule list and POSIX account/group inventory (no changes permitted in this release).
- Basic audit events: authentication result, denied actions, and agent calls.
- Unit/integration checks, CI, architecture and security documentation.

**Not included:** editing UFW, editing accounts, SSH keys, sudo, remote server management.

**DoD:** demonstrate login as admin and viewer, denial as ordinary Linux account, locked/expired account rejection, and API refusal of all mutation endpoints.

### v0.2.0 — firewall changes and Safe Apply
**Added**
- Validated UFW rule creation, change, and deletion; IPv4/IPv6, protocol, ports and CIDRs.
- Changeset preview and explicit validation, including probable SSH access impact.
- Host-side snapshot + independent scheduled rollback (survives browser disconnect and API process restart); explicit acknowledgement to cancel rollback.
- One in-flight pending change per host, with tamper-resistant change ID and complete audit record.

**DoD:** run on disposable VMs and prove rollback works after lost browser, timeout, and API/agent failure; document residual risks, including reboot/scheduler behavior and any UFW/Docker interactions.

### v0.3.0 — Linux account administration
**Added**
- Create/lock/unlock non-system accounts, manage supplementary groups, inspect shells/home directories and UID/GID.
- Carefully designed account removal with two-step confirmation, optional home retention, and explicit audit.
- Protected-account rules: no modifications to root/system users, last administrator, or currently executing privileged identity.
- Fresh authorization on each management operation to handle mid-session role/account changes.

**DoD:** a suite of isolated host tests verifies the protected-user and last-admin safeguards.

### v0.4.0 — SSH and sudo management
**Added**
- Per-account `authorized_keys` inventory/add/revoke, preserving file permissions and ownership.
- Review-based sudo group/policy changes; syntax validation with `visudo` for supported policy files.
- Recovery-first checks to preserve an alternative trusted administrator access path.

**DoD:** fail closed on invalid SSH/sudo changes, demonstrate recovery from a rejected modification, and log actor/target/before-and-after policy.

### v0.5.0 — exposure and audits
**Added**
- Docker published port visibility and warnings where Docker networking bypasses expected UFW rules.
- Listener inventory, search/filter and export for audit events, actionable configuration warnings.

**DoD:** container networking behavior is tested and UI explicitly labels findings as observations, not a security guarantee.

### v0.6.0 — installation and maintainability
**Added**
- Documented Ubuntu LTS installation/upgrade/remove flow, systemd hardening, lifecycle health endpoints, backups.
- Internationalized interface strings (EN/FA), contributor and release documentation.

**DoD:** clean-machine install, upgrade and rollback instructions validated on supported Ubuntu versions.

### v1.0.0 — single-host stable
**Added/changed**
- Close security gaps found during threat modeling; complete test coverage and documented recovery drill.
- Accessibility and reliability improvements. Freeze and document the supported API surface.

**DoD:** pass security checklist, e2e system tests, installation & restore drills, and publish complete signed/reproducible artifacts when feasible.

### Future after v1.0
- **v1.1.0**: optional nftables backend (not a promise to manage firewalld concurrently).
- **v1.2.0**: securely enrolled multi-server management, still without an arbitrary remote terminal.

## Explicit non-goals for v1.0
- Public-facing internet management panel by default.
- Arbitrary root shell, user-written scripts, or unrestricted command execution.
- Password storage or a separate database of Linux login passwords.
- Editing Docker's iptables/nftables rules directly.
- Cloud management, SSH proxying, or multi-host management.
- Guaranteeing prevention of all SSH lockouts; out-of-band recovery remains required for initial deployment.

## Definition of Done for **every** version

A version is not complete merely because its UI exists.

1. Scope and acceptance criteria checked off with evidence.
2. Unit and integration tests pass; host-affecting features pass isolated-VM end-to-end tests.
3. Security regression checks, least-privilege review, no known high-risk exploit path.
4. Installation/upgrade notes and documentation updated in **English and Persian**.
5. `CHANGELOG.md` finalized under a release heading with **Added / Changed / Fixed / Security / Breaking / Known issues** as applicable.
6. GitHub Actions green on the release commit, tagged `vX.Y.Z`, artifacts generated, GitHub Release with clear known limitations.

## Semantic Versioning / release changes

- **Patch** `0.x.y` or `1.x.y`: bug fixes, security patches, documentation fixes; **no new headline features**.
- **Minor** `0.x.0` before 1.0: a planned feature milestone, potentially with breaking adjustments explicitly documented.
- **Minor** `1.x.0` after 1.0: backwards-compatible features; incompatible API changes require a new major version.
- Every change must link to an issue/PR and a roadmap release target. A feature moves only after its stated security prerequisites are complete.
- If scope changes, update this roadmap **before release**, not retroactively. Mark completed work only after a tagged release exists.

---

## خلاصه نقشه راه فارسی

این نقشه راه، **برنامه توسعه** است و به معنی انتشار نسخه‌های جدول نیست.

| نسخه | امکانات و تغییرات |
| --- | --- |
| **0.1.0** | ساختار Angular و ASP.NET Core، ورود با PAM، نقش‌های گروهی لینوکس، نمایش فقط‌خواندنی UFW و کاربران و گروه‌ها، ثبت رخدادها، تست و CI |
| **0.2.0** | افزودن، ویرایش و حذف قانون UFW، بررسی اثر تغییر روی SSH، پیش‌نمایش تغییرات، بازگشت خودکار در صورت تایید نشدن، پشتیبانی IPv4/IPv6 |
| **0.3.0** | ایجاد و غیرفعال‌سازی کاربران محلی، مدیریت عضویت گروه‌ها، حذف کنترل‌شده با محافظت از مدیر اصلی و کاربران سیستمی |
| **0.4.0** | مدیریت کلیدهای SSH، دسترسی‌های sudo با اعتبارسنجی و حفاظت در برابر قطع مسیر مدیریت سرور |
| **0.5.0** | مشاهده پورت‌های منتشرشده Docker و سطح دسترسی واقعی شبکه، هشدارهای امنیتی و گزارش‌های ممیزی |
| **0.6.0** | نصب و ارتقا روی اوبونتو، سرویس‌های systemd، پشتیبان‌گیری و بازیابی، تکمیل ترجمه و مستندات |
| **1.0.0** | انتشار پایدار تک‌سروره پس از آزمون‌های امنیتی، بازیابی و پایداری |
| **1.1.0** | پشتیبانی اختیاری از nftables |
| **1.2.0** | مدیریت چند سرور با ارتباط رمزگذاری‌شده و احراز هویت‌شده |

**قانون انتشار:** هر نسخه باید فهرست دقیق قابلیت‌های جدید، تغییرات، اصلاح خطاها، موارد امنیتی و محدودیت‌های شناخته‌شده داشته باشد. تا زمانی که تست‌ها و معیارهای پذیرش کامل نشده‌اند، آن نسخه منتشرشده محسوب نمی‌شود.
