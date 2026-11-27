# WallForge v0.1.0-rc.1 — UNPUBLISHED PRE-RELEASE

This Linux x64 review candidate is not a stable release. No final tag, merge, or GitHub Release is created by preparing these files.

The package contains a self-contained ASP.NET Core API, Angular Production UI, native PAM verifier, exact sudoers/PAM configuration, Docker-only installer, optional unverified systemd unit, installation instructions, license, and build provenance. It includes the original owner-approved horizontal logo, symbol, favicon and icons without redesign or pixel edits.

Capabilities are read-only: PAM login, OS-group Administrator/Viewer authorization, UFW status/rules, local users/groups, and bounded administrator activity. Neither role can mutate firewall or accounts. Sessions expire after20 minutes without sliding renewal; the UI detects expiration on its next API request. The UFW reader has an independent privileged deadline, and only two exact sudo operations are permitted.

Upgrade API and sudoers together. The Docker installer preserves an existing wfadmin account/password; new installations prompt for a throwaway password. Preserve matching prior binaries/startup/policy for rollback. No migration from a stable release exists because no stable version has shipped.

Test outcomes and exact artifact SHA256 are delivered separately in TEST_REPORT_v0.1.0.md and SHA256SUMS.txt. systemd runtime, real reboot/key continuity, durable journald, VM-specific PAM policy and active IPv4/IPv6 UFW acceptance remain BLOCKED when unsupported by Docker. Independent security/PR approval is outstanding. Production TLS acceptance and unavailable browser platforms are not asserted. Use only an isolated trusted management environment, not a public production server.
