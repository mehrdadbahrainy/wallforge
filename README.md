# WallForge

**Safe. Simple. Open.**

WallForge is an early-stage, open-source web application for **safe Ubuntu firewall and local Linux access management**.

> [!WARNING]
> **Planning stage — no installable release yet.** WallForge is not audited or ready for production. Never expose an early build directly to the public internet.

## Architecture (planned)

- **Frontend:** Angular + TypeScript
- **API:** ASP.NET Core (unprivileged service)
- **Privileged host agent:** isolated, minimal allowlisted operations; no arbitrary shell commands
- **Firewall:** UFW initially; nftables backend planned after v1.0
- **Authentication:** Linux PAM, with independent authorization via `wallforge-admin` and `wallforge-viewer` OS groups
- **Deployment:** single Ubuntu host at first

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

1. WallForge web/API services must run unprivileged and should be accessible only on trusted networks.
2. Privileged agent must accept only specifically validated operations through a restricted local transport.
3. Firewall writes must have a **host-level rollback mechanism independent of the browser session**; require explicit confirmation.
4. Refuse unsafe modifications of root/system accounts or the last administrative login path.
5. Treat container-published ports as potentially bypassing UFW filtering; show their effective exposure rather than claiming UFW covers them.
6. Never accept arbitrary shell execution, raw sudo commands, or user-supplied executable strings.

## Release policy

We follow SemVer and keep [CHANGELOG.md](CHANGELOG.md) using **Added / Changed / Fixed / Security / Breaking / Known issues**. A release is only marked complete after tests and acceptance criteria pass. No release is asserted until a tagged artifact and release notes exist.

---

## فارسی

**وال‌فورج** یک پروژه متن‌باز برای مدیریت امن فایروال اوبونتو و دسترسی کاربران محلی لینوکس است.

**وضعیت فعلی: برنامه‌ریزی، بدون نسخه قابل نصب.**

- رابط کاربری: Angular و TypeScript
- API: ASP.NET Core
- احراز هویت: کاربران لینوکس از طریق PAM، بدون پایگاه داده رمز عبور مستقل
- مجوزها: گروه‌های `wallforge-admin` و `wallforge-viewer`
- اولین هدف: مشاهده وضعیت UFW و کاربران/گروه‌های محلی
- هدف بعدی: تغییر امن قوانین فایروال با بازگشت خودکار در صورت تایید نشدن
- قابلیت‌های مدیریت کاربران، کلیدهای SSH و دسترسی sudo در نسخه‌های بعدی

برای مشاهده امکانات دقیق هر نسخه، فایل [نقشه راه](docs/ROADMAP.md) را ببینید.
