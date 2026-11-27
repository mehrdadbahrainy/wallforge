#!/usr/bin/env bash
set -euo pipefail
[[ $(id -u) == 0 && -f /.dockerenv ]] || { echo 'Run only as root inside the disposable Docker lab.'; exit 1; }
[[ $(uname -m) == x86_64 ]] || { echo 'This preview package requires x86_64 Linux.'; exit 1; }
cd "$(dirname "$(realpath "$0")")"
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y --no-install-recommends ca-certificates sudo ufw libpam0g libpam-modules libpam-runtime openssl libicu74 libssl3t64 zlib1g libgssapi-krb5-2
apt-get clean
rm -rf /var/lib/apt/lists/*
install -d -m 0755 /opt/wallforge/api /usr/local/libexec
cp -a api/. /opt/wallforge/api/
chown -R root:root /opt/wallforge/api
chmod 0755 /opt/wallforge/api/WallForge.Api
groupadd --system --force wallforge-admin
groupadd --system --force wallforge-viewer
id wallforge-api >/dev/null 2>&1 || useradd --system --user-group --home-dir /var/lib/wallforge --shell /usr/sbin/nologin wallforge-api
if ! id wfadmin >/dev/null 2>&1; then
  useradd -m -s /bin/bash wfadmin
  echo 'Set a unique throwaway password for the new wfadmin account.'
  passwd wfadmin
else
  echo 'Existing wfadmin account and password preserved.'
fi
usermod -aG wallforge-admin wfadmin
install -o root -g root -m 0755 wallforge-pam-auth /usr/local/libexec/wallforge-pam-auth
install -o root -g root -m 0644 pam-wallforge /etc/pam.d/wallforge
install -o root -g root -m 0440 sudoers-wallforge /etc/sudoers.d/wallforge
visudo -cf /etc/sudoers.d/wallforge
install -d -o wallforge-api -g wallforge-api -m 0700 /var/lib/wallforge /var/lib/wallforge/keys
install -d -o root -g wallforge-api -m 0750 /opt/wallforge/tls
if [[ ! -f /opt/wallforge/tls/localhost.key || ! -f /opt/wallforge/tls/localhost.crt ]]; then
  openssl req -x509 -newkey rsa:2048 -nodes -days 14 -subj '/CN=localhost' -addext 'subjectAltName=DNS:localhost,IP:127.0.0.1' -keyout /opt/wallforge/tls/localhost.key -out /opt/wallforge/tls/localhost.crt
fi
chown root:wallforge-api /opt/wallforge/tls/localhost.key /opt/wallforge/tls/localhost.crt
chmod 0640 /opt/wallforge/tls/localhost.key
chmod 0644 /opt/wallforge/tls/localhost.crt
cat >/opt/wallforge/start.sh <<'START'
#!/usr/bin/env bash
set -euo pipefail
umask 077
ulimit -c 0
cd /opt/wallforge/api
exec runuser -u wallforge-api -- env ASPNETCORE_URLS=https://0.0.0.0:5443 ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_Kestrel__Certificates__Default__Path=/opt/wallforge/tls/localhost.crt ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/opt/wallforge/tls/localhost.key WALLFORGE_KEYS_DIR=/var/lib/wallforge/keys /opt/wallforge/api/WallForge.Api
START
chmod 0755 /opt/wallforge/start.sh
echo 'Run: bash /opt/wallforge/start.sh ; browse https://localhost:5443'
