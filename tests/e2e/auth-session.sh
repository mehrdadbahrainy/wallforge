#!/usr/bin/env bash
# Isolated Ubuntu CI only. Never execute on a production server.
set -euo pipefail
umask 077

base="https://127.0.0.1:5443"
admin_jar="$(mktemp)"
viewer_jar="$(mktemp)"
rm -f "$admin_jar" "$viewer_jar"
password="$(openssl rand -hex 22)"
printf '%s:%s\n' wf-ci-admin "$password" | sudo chpasswd
printf '%s:%s\n' wf-ci-viewer "$password" | sudo chpasswd

openssl req -x509 -nodes -newkey rsa:2048 -days 1 -keyout /tmp/wallforge-ci.key \
  -out /tmp/wallforge-ci.crt -subj '/CN=localhost' >/dev/null 2>&1
openssl pkcs12 -export -out /tmp/wallforge-ci.pfx -inkey /tmp/wallforge-ci.key \
  -in /tmp/wallforge-ci.crt -passout pass:wallforge-ci >/dev/null 2>&1
sudo chown wallforge-api:wallforge-api /tmp/wallforge-ci.pfx
sudo chmod 0600 /tmp/wallforge-ci.pfx
sudo mkdir -p /tmp/wallforge-ci-keys
sudo chown wallforge-api:wallforge-api /tmp/wallforge-ci-keys
sudo chmod 0700 /tmp/wallforge-ci-keys

(
  cd /tmp/wallforge-api
  exec sudo -u wallforge-api env \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=https://127.0.0.1:5443 \
    ASPNETCORE_Kestrel__Certificates__Default__Path=/tmp/wallforge-ci.pfx \
    ASPNETCORE_Kestrel__Certificates__Default__Password=wallforge-ci \
    WALLFORGE_KEYS_DIR=/tmp/wallforge-ci-keys \
    /usr/bin/dotnet /tmp/wallforge-api/WallForge.Api.dll
) >/tmp/wallforge-ci-api.log 2>&1 &
api_pid=$!
cleanup() {
  kill "$api_pid" 2>/dev/null || true
  rm -f "$admin_jar" "$viewer_jar" /tmp/wallforge-ci.key /tmp/wallforge-ci.crt
  sudo rm -f /tmp/wallforge-ci.pfx
}
trap cleanup EXIT

healthy=0
for attempt in $(seq 1 40); do
  if curl --silent --insecure --fail "$base/api/health" | grep -q '"status":"ok"'; then
    healthy=1
    break
  fi
  sleep 1
done
if [ "$healthy" -ne 1 ]; then
  cat /tmp/wallforge-ci-api.log
  echo "API did not start over HTTPS" >&2
  exit 1
fi

http_code() {
  curl --silent --insecure --output /dev/null --write-out '%{http_code}' "$@"
}
csrf_token() {
  local jar="$1"
  curl --fail --silent --insecure -b "$jar" -c "$jar" "$base/api/auth/csrf" \
    | python3 -c 'import json,sys; print(json.load(sys.stdin)["token"])'
}

test "$(http_code "$base/api/host/firewall")" = "401"
test "$(http_code "$base/api/auth/csrf")" = "200"
test "$(http_code -X POST "$base/api/auth/login" -H 'Content-Type: application/json' \
  --data '{"userName":"wf-ci-admin","password":"wrong"}')" = "400"
echo "PASS anonymous endpoints require session and CSRF"

csrf="$(csrf_token "$admin_jar")"
status="$(http_code -b "$admin_jar" -c "$admin_jar" \
  -X POST "$base/api/auth/login" -H 'Content-Type: application/json' \
  -H "X-WallForge-CSRF: $csrf" \
  --data "{\"userName\":\"wf-ci-admin\",\"password\":\"$password\"}")"
test "$status" = "200"
admin_me="$(curl -fsSk -b "$admin_jar" "$base/api/auth/me")"
echo "$admin_me" | grep -q '"role":"admin"'
test "$(http_code -b "$admin_jar" "$base/api/audit")" = "200"
test "$(http_code -b "$admin_jar" "$base/api/host/users")" = "200"
test "$(http_code -b "$admin_jar" "$base/api/host/firewall")" = "200"
test "$(http_code -b "$admin_jar" -X POST "$base/api/host/firewall")" = "405"
echo "PASS admin HTTPS authentication and read-only host views; mutations blocked"

sudo gpasswd -d wf-ci-admin wallforge-admin >/dev/null
test "$(http_code -b "$admin_jar" "$base/api/auth/me")" = "401"
sudo usermod -aG wallforge-admin wf-ci-admin
echo "PASS mid-session group revocation"

viewer_csrf="$(csrf_token "$viewer_jar")"
status="$(http_code -b "$viewer_jar" -c "$viewer_jar" \
  -X POST "$base/api/auth/login" -H 'Content-Type: application/json' \
  -H "X-WallForge-CSRF: $viewer_csrf" \
  --data "{\"userName\":\"wf-ci-viewer\",\"password\":\"$password\"}")"
test "$status" = "200"
test "$(http_code -b "$viewer_jar" "$base/api/audit")" = "403"
test "$(http_code -b "$viewer_jar" "$base/api/host/users")" = "200"
viewer_csrf="$(csrf_token "$viewer_jar")"
test "$(http_code -b "$viewer_jar" -c "$viewer_jar" -X POST "$base/api/auth/logout" \
  -H "X-WallForge-CSRF: $viewer_csrf" -H 'Content-Type: application/json' --data '{}')" = "204"
test "$(http_code -b "$viewer_jar" "$base/api/auth/me")" = "401"
echo "PASS viewer role, identity-bound CSRF refresh, and logout"

