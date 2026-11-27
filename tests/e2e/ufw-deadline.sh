#!/usr/bin/env bash
# Disposable GitHub Ubuntu runner only: replaces the executable with a harmless hang fixture.
set -euo pipefail
[[ "${GITHUB_ACTIONS:-}" == true ]] || { echo 'Only the disposable GitHub runner is supported.'; exit 1; }
backup="$(mktemp)"
sudo cp /usr/sbin/ufw "$backup"
cleanup() {
  sudo cp "$backup" /usr/sbin/ufw
  sudo chmod 0755 /usr/sbin/ufw
  sudo rm -f "$backup" /tmp/wallforge-hanging-ufw.pid
}
trap cleanup EXIT
sudo tee /usr/sbin/ufw >/dev/null <<'FIXTURE'
#!/usr/bin/env bash
[[ "$*" == 'status numbered' ]] || exit 1
echo "$$" >/tmp/wallforge-hanging-ufw.pid
trap '' TERM
sleep 60
FIXTURE
sudo chmod 0755 /usr/sbin/ufw
sudo -u wallforge-api /usr/bin/dotnet /tmp/wallforge-integration/WallForge.LinuxIntegration.dll --ufw-timeout
sudo rm -f /tmp/wallforge-hanging-ufw.pid
sudo -u wallforge-api /usr/bin/dotnet /tmp/wallforge-integration/WallForge.LinuxIntegration.dll --ufw-cancel
# This command must be denied by sudoers; it must never be executed as root.
if sudo -u wallforge-api sudo -n /usr/bin/timeout 6s /usr/bin/id >/dev/null 2>&1; then
  echo 'FAIL arbitrary timeout command was permitted'; exit 1
fi
echo 'PASS altered timeout arguments remain forbidden'
