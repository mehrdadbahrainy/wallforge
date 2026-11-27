# Active UFW acceptance in the existing Docker lab

Do not run on a real host or production container. Current manual target is wallforge-local-lab-ubuntu-1 with existing NET_ADMIN, its own network namespace, no privileged mode and localhost-only5443 publishing. No capabilities or network settings need changing. UFW must be configured by the authorized lab operator; these API/PAM suites do not activate/change firewall rules.

Copy local-container.py and pam-policy-container.py into the lab. API suite needs the existing public TLS CA and self-contained harness at /tmp/wallforge-validation-integration/WallForge.LinuxIntegration. Run as root inside the lab:

```sh
WF_EXPECT_UFW_STATUS=active python3 /tmp/local-container.py
WALLFORGE_DISPOSABLE_LAB=1 python3 /tmp/pam-policy-container.py
```

The first suite compares complete real numbered CLI/API inventories, requires both IPv4/IPv6 rows in active mode, checks Viewer read access and preserves the current /etc/ufw hash. It defaults to inactive for older fixtures. It creates/removes wf-val-* accounts. The second checks existing Ubuntu PAM policies with a generated throwaway wf-act-policy account and removes it afterward. Neither exports passwords/cookies. Failed result rows return nonzero.

Actual ingress enforcement, UFW reload and real API-process restart were separately exercised in this approved lab; evidence is under docs/evidence/v0.1.0/active-*. External IPv6 traffic is not covered by merely listing v6 rules. PID1docker-init does not support real systemd/reboot/journal acceptance.
