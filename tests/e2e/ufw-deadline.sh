#!/usr/bin/env bash
# Disposable GitHub Ubuntu runner or explicitly identified Docker lab only: replaces the executable with a harmless hang fixture.
set -euo pipefail
[[ "${GITHUB_ACTIONS:-}" == true || ( -f /.dockerenv && "${WALLFORGE_DISPOSABLE_LAB:-}" == 1 ) ]] || { echo 'Use only a disposable GitHub runner or explicitly identified Docker lab.'; exit 1; }
harness=(/usr/bin/dotnet /tmp/wallforge-integration/WallForge.LinuxIntegration.dll)
if [[ -f /.dockerenv && -n "${WF_INTEGRATION_EXECUTABLE:-}" ]]; then
  harness=("$WF_INTEGRATION_EXECUTABLE")
fi
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
sudo -u wallforge-api "${harness[@]}" --ufw-timeout
sudo rm -f /tmp/wallforge-hanging-ufw.pid
sudo -u wallforge-api "${harness[@]}" --ufw-cancel
# This command must be denied by sudoers; it must never be executed as root.
if sudo -u wallforge-api sudo -n /usr/bin/timeout 6s /usr/bin/id >/dev/null 2>&1; then
  echo 'FAIL arbitrary timeout command was permitted'; exit 1
fi
echo 'PASS altered timeout arguments remain forbidden'
