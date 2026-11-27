# WallForge v0.1.0 HTTP API

All user-specific routes require a `__Host-wallforge-session` HTTPS-only cookie, Linux PAM authentication, and membership in `wallforge-admin` or `wallforge-viewer`. Membership is rechecked on every request.

| Method | Path | Permissions | Description |
|---|---|---|---|
| GET | `/api/health` | public | Service health without host details |
| GET | `/api/auth/csrf` | public, rate limited | Returns one-time antiforgery request token |
| POST | `/api/auth/login` | public, rate limited + CSRF | Body `{userName,password}`; returns `{userName,role}` |
| GET | `/api/auth/me` | authenticated | Current account and up-to-date role |
| POST | `/api/auth/logout` | authenticated + CSRF | Terminates the session |
| GET | `/api/host/firewall` | admin or viewer | UFW availability, active/inactive status, raw numbered rules |
| GET | `/api/host/users` | admin or viewer | Read-only local users and groups |
| GET | `/api/audit` | admin only | Bounded in-memory activity records |

No user or firewall mutation endpoints exist in this version. `POST /api/host/firewall` and similar requests are unsupported.

## Antiforgery flow

```text
GET /api/auth/csrf -> {"token":"..."}
POST /api/auth/login
  Cookie: __Host-wallforge-xsrf=...
  X-WallForge-CSRF: ...
  Content-Type: application/json
  {"userName":"alice","password":"..."}
```

Use same-origin HTTPS access (or a trusted local TLS reverse proxy). Avoid recording passwords in proxy logs or command history.
