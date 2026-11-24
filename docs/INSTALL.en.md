# WallForge v0.1.0 development installation (Ubuntu)

> **Developer preview only — not a public production release.** All host-affecting tests must use a disposable Ubuntu VM or a machine with out-of-band console access. This version is read-only and must never be opened directly to the public Internet.

## Requirements

- Ubuntu 24.04 LTS or another explicitly tested Ubuntu release
- .NET 10 SDK/runtime and Node.js 24, npm
- `ufw`, `sudo`, `libpam0g` available
- A local non-root Ubuntu user with a password and UID >= 1000
- A **trusted HTTPS reverse proxy** on the same host, or run the API with a correctly configured HTTPS endpoint for local development

## Build

```bash
git clone https://github.com/mehrdadbahrainy/wallforge.git
cd wallforge
git checkout feature/v0.1.0-foundation

dotnet publish src/WallForge.Api/WallForge.Api.csproj -c Release -o ./out/api
cd frontend
npm install
npm run build
cd ..
mkdir -p out/api/wwwroot
cp -a frontend/dist/wallforge/browser/. out/api/wwwroot/
```

## Ubuntu identity and privileges

Create two **WallForge-specific Linux groups** and grant access only to accounts you intend to authorize:

```bash
sudo groupadd --system --force wallforge-admin
sudo groupadd --system --force wallforge-viewer
sudo usermod -aG wallforge-admin YOUR_UBUNTU_USERNAME
```

A member of `wallforge-admin` can view the audit view; `wallforge-viewer` has inventory-only access. **Neither role can modify host state in v0.1.0.** Linux root and system users are rejected by the application. Local memberships in `/etc/group` and local accounts in `/etc/passwd` are the only role/account inventory sources in v0.1.0. This does not support LDAP/AD identity in this release.

Install the PAM service used to authenticate a local Ubuntu username/password and check account status:

```bash
sudo install -o root -g root -m 0644 ops/pam.d/wallforge /etc/pam.d/wallforge
```

Create the dedicated **unprivileged API** identity and a tightly scoped sudoers rule for UFW status **only**:

```bash
sudo useradd --system --home-dir /var/lib/wallforge --shell /usr/sbin/nologin wallforge-api
sudo install -o root -g root -m 0440 ops/sudoers.d/wallforge /etc/sudoers.d/wallforge
sudo visudo -cf /etc/sudoers.d/wallforge
```

The API never receives arbitrary shell/command arguments from clients. Its UFW inspection command is exactly:
`/usr/bin/sudo -n /usr/sbin/ufw status numbered`.

> PAM authentication from a non-root application depends on the host PAM stack and must be verified using actual local Ubuntu accounts before calling this release ready. Do not weaken `/etc/shadow` permissions.

## Run behind trusted HTTPS

For a local developer experiment, run the API with a valid .NET developer certificate, e.g.:

```bash
ASPNETCORE_URLS=https://localhost:5443 dotnet out/api/WallForge.Api.dll
```

Visit `https://localhost:5443/` after trusting the development certificate. All cookies are `Secure`, `HttpOnly`, `SameSite=Strict`, and login/logout requests require an antiforgery token. Do not try to authenticate over plain HTTP.

For a systemd deployment, copy `out/api` to `/opt/wallforge/api`, install `ops/systemd/wallforge-api.service` to `/etc/systemd/system`, then put a trusted localhost reverse proxy with TLS in front of `http://127.0.0.1:5080`. Ensure it forwards `Host` and `X-Forwarded-Proto` from a **trusted loopback** proxy and restrict inbound access to your management VPN/network.

```bash
sudo mkdir -p /opt/wallforge
sudo cp -a out/api /opt/wallforge/api
sudo chown -R root:root /opt/wallforge/api
sudo install -o root -g root -m 0644 ops/systemd/wallforge-api.service /etc/systemd/system/wallforge-api.service
sudo systemctl daemon-reload
sudo systemctl enable --now wallforge-api
sudo journalctl -u wallforge-api -f
```

The systemd unit creates `/var/lib/wallforge` and persists cookie protection keys under `/var/lib/wallforge/keys`. Only the API identity must have write access. The unit cannot enable `NoNewPrivileges` while using restricted sudo. This is a **v0.1 limitation requiring security review**; a dedicated host agent will replace the sudo inspection boundary when safe firewall writes are implemented.

## Verification (release blockers)

1. Run `dotnet run --project tests/WallForge.SmokeTests/WallForge.SmokeTests.csproj` and `cd frontend && npm run typecheck && npm run build`.
2. Test PAM password success/failure with a disposable non-root Ubuntu account.
3. Confirm root, expired/locked accounts, service users and non-members cannot log in.
4. Confirm admin and viewer can view their permitted routes, while viewers cannot view audit events.
5. Remove a user's WallForge group membership and confirm authorization is revoked on a subsequent request.
6. Check that all firewall/user endpoints are GET-only. Confirm no authenticated action changes UFW state.
7. Confirm HTTPS, CSRF, cookies, login rate limiting, restart behavior and UFW status on a real Ubuntu host.
8. Record test evidence in Google Drive project tracker and the release PR **before tagging**.

## Known limitations

- No write features, Safe Apply, account changes, sudo/SSH management or Docker exposure detection.
- No LDAP or AD directory account source. UFW output is presented as plain read-only lines.
- Audit UI shows a bounded **in-memory** event buffer; server logs are emitted to the host journal. Logging persistence depends on the host's journald configuration.
- A password lock imposed after sign-in may not revoke an existing short-lived session immediately; OS-group removal is rechecked on every authenticated request. Account lockout semantics require real PAM stack testing.
- HTTPS reverse proxy, PAM stack, sudoers and correct Linux group membership are local admin responsibilities. Do not expose WallForge outside a trusted management network.
