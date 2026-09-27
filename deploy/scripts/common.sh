# shellcheck shell=bash
# Shared helpers for the deployment scripts (sourced, not executed).
set -euo pipefail

DEPLOY_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$DEPLOY_DIR"

if [[ ! -f .env ]]; then
  echo "error: $DEPLOY_DIR/.env is missing (copy .env.example and fill it in)" >&2
  exit 1
fi

# Reads one value from .env without sourcing it (values may contain shell metacharacters).
env_value() {
  local value
  value=$(grep -E "^$1=" .env | tail -n1 | cut -d= -f2- || true)
  echo "${value:-${2:-}}"
}

# Updates (or appends) one key in .env.
set_env_value() {
  if grep -qE "^$1=" .env; then
    local tmp
    tmp=$(mktemp)
    awk -v key="$1" -v value="$2" -F= 'BEGIN { OFS = "=" } $1 == key { print key, value; next } { print }' .env > "$tmp"
    cat "$tmp" > .env
    rm -f "$tmp"
  else
    echo "$1=$2" >> .env
  fi
}

# sqlcmd inside the SQL Server container; the sa password never leaves the container's environment.
sqlcmd() {
  docker compose exec -T sqlserver bash -c \
    '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -b -h -1 -W "$@"' _ "$@"
}

log() { printf '\n==> %s\n' "$*"; }
