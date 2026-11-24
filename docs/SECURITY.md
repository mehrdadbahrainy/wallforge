# WallForge — v0.1 security model

**Scope:** a single local Ubuntu host, read-only operations only. The service is experimental and not security audited. Untrusted or public network exposure is forbidden for this phase.

## Trust boundaries

1. Angular web UI: untrusted browser. No privileged direct operations. Never trust client role flags.
2. ASP.NET Core API: dedicated unprivileged Linux identity `wallforge-api`. Authenticates with PAM via `libpam`. Only emits HTTP-only session cookies (no JWT/localStorage credentials).
3. PAM service `/etc/pam.d/wallforge`: authenticates against host configuration. Only one hidden password prompt is implemented in this phase. PAM account-management checks are made after successful password verification.
4. Account and role inventory: only `/etc/passwd` and `/etc/group` local entries (not LDAP/NSS beyond those files). Users must have UID >= 1000 and be explicitly in `wallforge-admin` or `wallforge-viewer`; root is denied.
5. UFW inspector: one fixed, no-shell subprocess `sudo -n /usr/sbin/ufw status numbered`, controlled by exact argument-matched sudoers policy. The role cannot run any other sudo command. The command accepts no browser-provided arguments.
6. Journald: receives structured audit event names/actors, not passwords. Bounded UI audit buffer is ephemeral.

## Authentication/session invariants

- No independently stored passwords. PAM is authoritative on login.
- Root and service account sign-in denied regardless of password.
- Missing Linux group = no session, even if PAM accepts the account.
- Revalidate local group membership for **every** authenticated API operation.
- Cookie: Secure, HttpOnly, SameSite=Strict, session expiry after 20 minutes, no sliding renewal.
- All present POST routes (login and logout) require explicit antiforgery header.
- Login limited to five attempts per minute per remote IP. Avoid changing reverse-proxy forwarded-header trust without review.
- No host-state mutation endpoints. Role 'admin' in this release only expands read-only audit visibility.

## Threats and remaining risks

- The restricted sudoers boundary still depends on setuid. As a consequence, `NoNewPrivileges` cannot be enabled on the API service. Revisit this as part of the dedicated privileged agent design. Never grant a wildcard sudoers command.
- User account state after a successful login is not re-authenticated on every request. Group revocation is immediate; password-lock changes require further integration tests and may not terminate an existing session until expiry.
- Default PAM configurations vary. MFA/multi-prompt PAM is not implemented; unsupported challenges must fail closed.
- The PAM native interop marshals request secrets through managed memory. Passwords are never persisted or logged. Memory hygiene and PAM behavior require further review.
- Local `/etc/passwd`/`/etc/group` inventory omits LDAP accounts and may omit other NSS-resolved identities.
- UFW rules are observations, not an assertion that Docker-published ports are filtered.
- In-memory audit UI resets on restart. Systemd/journald retention must be configured separately.
- Secrets, TLS certificates, sudoers file and service identity must be provisioned safely by the host administrator.

## Security acceptance checklist before v0.1 release

- [ ] Linux PAM login/expiry/lockout tested on supported Ubuntu releases
- [ ] Non-member, viewer, admin, root, system account matrix verified
- [ ] Per-request group revocation tested with active sessions
- [ ] Confirmed no write endpoint and no arbitrary command execution
- [ ] CSRF token, same-site cookies, HTTPS, login throttling smoke and abuse tests
- [ ] Rate limits observed behind the trusted proxy
- [ ] `sudo -l -U wallforge-api` confirms exactly the intended command
- [ ] Host UFW status read correctly on isolated test VM
- [ ] Persistent cookie key permissions and journal review verified
- [ ] Independent security review and release signoff completed

Vulnerability reports: please open a private security advisory rather than a public exploit issue.
