# WallForge

**Safe. Simple. Open.**

WallForge is an early-stage, open-source web application for **safe Ubuntu firewall and local Linux access management**.

> [!WARNING]
> **v0.1.0 development implementation is under review, NOT RELEASED.** GitHub Actions builds Angular and .NET and passes Ubuntu 24.04 PAM/UFW and authenticated HTTPS API integration tests, but manual installation, browser testing and a security review remain mandatory. Never expose an early build directly to the public internet.

## Architecture (planned)

- **Frontend:** Angular + TypeScript
- **API:** ASP.NET Core (unprivileged service)
- **Host read boundary (v0.1):** unprivileged API invokes two fixed, argument-exact root operations: a tiny native PAM verifier and read-only `sudo -n /usr/bin/timeout --signal=TERM --kill-after=1s 6s /usr/sbin/ufw status numbered`; a dedicated privileged host agent is planned for safe writes later
- **Firewall:** UFW initially; nftables backend planned after v1.0
- **Authentication:** Linux PAM, with independent authorization via `wallforge-admin` and `wallforge-viewer` OS groups
- **Deployment:** single Ubuntu host at first, trusted HTTPS management network only

Local Linux accounts authenticate to WallForge through PAM; there is **no second password database**. PAM login does not by itself confer WallForge access. Root login is disabled by default, access is denied unless explicitly authorized, and privileges must be checked again for sensitive requests.

## Planned releases

| Version | Goal | Highlights |
| --- | --- | --- |
| 0.1.0 | Secure read-only foundation | PAM login, OS-group roles, UFW visibility, local users/groups inventory, Angular UI, audit/CI |
| 0.2.0 | Safe firewall editing | UFW rules, preflight checks, timed rollback, confirmation, IPv4/IPv6 |
| 0.3.0 | User and group administration | Create/lock/unlock users, group membership, protected-account safeguards |
| 0.4.0 | SSH and sudo access | Authorized SSH keys, controlled sudo access, safety validation |
| 0.5.0 | Security visibility | Docker published-port visibility, effective network exposure warnings, audit filtering |
| 0.6.0 | Deployment and operations | Ubuntu packaging, systemd integration, backup/restore, upgrade tests |
| 1.0.0 | Stable single-host release | Security review, recovery drills, hardening, complete tested docs |
| 1.1.0 | Firewall backend expansion | nftables backend after parity/security review |
| 1.2.0 | Multi-host operations | Authenticated agent connectivity, per-host policies and health |

These versions are **targets, not shipped features**. The complete acceptance criteria and order of work are in [ROADMAP.md](docs/ROADMAP.md).

## Security priorities

1. WallForge web/API services must run unprivileged and should be accessible only on trusted HTTPS networks.
2. In v0.1, a restricted sudoers policy allows only a native PAM verifier and one read-only UFW status command. A dedicated privileged host agent is required before any host mutation.
3. Firewall writes must have a **host-level rollback mechanism independent of the browser session**; require explicit confirmation.
4. Refuse unsafe modifications of root/system accounts or the last administrative login path.
5. Treat container-published ports as potentially bypassing UFW filtering; show their effective exposure rather than claiming UFW covers them.
6. Never accept arbitrary shell execution, raw sudo commands, or user-supplied executable strings.

## Release policy

We follow SemVer and keep [CHANGELOG.md](CHANGELOG.md) using **Added / Changed / Fixed / Security / Breaking / Known issues**. A release is only marked complete after tests and acceptance criteria pass. No release is asserted until a tagged artifact and release notes exist.

---

## فارسی

**وال‌فورج** یک پروژه متن‌باز برای مدیریت امن فایروال اوبونتو و دسترسی کاربران محلی لینوکس است.

**وضعیت فعلی: پیش‌نمایش توسعه 0.1.0، در حال بررسی و بدون انتشار تاییدشده.**

- رابط کاربری: Angular و TypeScript
- API: ASP.NET Core
- احراز هویت: کاربران لینوکس از طریق PAM، بدون پایگاه داده رمز عبور مستقل
- مجوزها: گروه‌های `wallforge-admin` و `wallforge-viewer`
- اولین هدف: مشاهده وضعیت UFW و کاربران/گروه‌های محلی
- هدف بعدی: تغییر امن قوانین فایروال با بازگشت خودکار در صورت تایید نشدن
- قابلیت‌های مدیریت کاربران، کلیدهای SSH و دسترسی sudo در نسخه‌های بعدی

برای مشاهده امکانات دقیق هر نسخه، فایل [نقشه راه](docs/ROADMAP.md) را ببینید.


## v0.1.0 development checkout

The source implementation is available on [feature/v0.1.0-foundation](https://github.com/mehrdadbahrainy/wallforge/tree/feature/v0.1.0-foundation) and tracked in [PR #2](https://github.com/mehrdadbahrainy/wallforge/pull/2). **Do not use it on a public production server.**

- [English installation and Ubuntu verification](docs/INSTALL.en.md)
- [راهنمای نصب و تست فارسی](docs/INSTALL.fa.md)
- [Security model and known limitations](docs/SECURITY.md)
- [HTTP API](docs/API.md)
- [Contributing](CONTRIBUTING.md)

Quick safe build:

```bash
dotnet build src/WallForge.Api/WallForge.Api.csproj -c Release
dotnet run -c Release --project tests/WallForge.SmokeTests/WallForge.SmokeTests.csproj
cd frontend && npm install && npm run typecheck && npm run build
```

**Verified CI:** [Ubuntu PAM/UFW and authenticated HTTPS API tests](https://github.com/mehrdadbahrainy/wallforge/actions/runs/45483913728) pass on a disposable Ubuntu 24.04 runner. Manual install and production security validation remain open.

**Current 0.1 read-only capabilities:** PAM authentication, local OS-group authorization, view UFW status/numbered rules, view local users/groups, view recent activity as admin. No firewall/users mutations exist. The real Ubuntu integration and release checklist are intentionally pending.

## v0.1.0 local validation and review candidate

Local validation evidence and release blockers: [test report](docs/TEST_REPORT_v0.1.0.md), [resolved/pending issues](docs/ISSUES_v0.1.0.md), [release checklist](docs/RELEASE_CHECKLIST_v0.1.0.md), [CI status](docs/CI_STATUS_v0.1.0.md), and [unpublished candidate notes](docs/RELEASE_NOTES_v0.1.0.md). The owner-approved logo is integrated byte-for-byte. The Production v0.1.0-rc.1 Linux x64 package was installed in the existing isolated Ubuntu Docker lab:87 automated/package checks and13 Chrome UI scenarios passed. See [RC installation guide](docs/INSTALL_v0.1.0-rc.1.fa.md). **Pre-release prepared, unpublished; stable release blocked by independent review and real Ubuntu VM acceptance.**
