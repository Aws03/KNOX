#!/usr/bin/env bash
# Zero-surprise deployment on the host:
#   1. start/verify SQL Server       2. back up the database        3. pull the new images
#   4. migrate (old version still serving)                           5. switch containers, wait for health
#   6. on failure: switch back to the previous images (the backup from step 2 is kept for a manual restore)
# Usage: scripts/deploy.sh [API_VERSION] [WEB_VERSION]     (default: the versions in .env)
# Migrations must stay backward-compatible with the previous release (expand first, contract in a later
# release) so that rolling the application back never requires restoring the database.
source "$(dirname "$0")/common.sh"

previous_api=$(env_value KNOX_API_VERSION)
previous_web=$(env_value KNOX_WEB_VERSION)
api="${1:-$previous_api}"
web="${2:-$previous_web}"
export KNOX_API_VERSION="$api" KNOX_WEB_VERSION="$web"

log "Deploying api=$api web=$web (currently api=$previous_api web=$previous_web)"

if [[ "${KNOX_SKIP_PULL:-0}" != 1 ]]; then   # KNOX_SKIP_PULL=1: images were built/loaded locally
  log "Pulling images"
  docker compose pull --quiet --ignore-buildable
  docker compose --profile tools pull --quiet migrate
fi

log "Starting the database"
docker compose up -d --wait --wait-timeout 180 sqlserver

"$DEPLOY_DIR/scripts/backup.sh" "pre-deploy-$api"

log "Migrating the database"
docker compose run --rm migrate

log "Switching to the new version"
set_env_value KNOX_API_VERSION "$api"
set_env_value KNOX_WEB_VERSION "$web"
if ! docker compose up -d --wait --wait-timeout 240 --remove-orphans; then
  echo "error: the new version did not become healthy; rolling back the application" >&2
  docker compose logs --tail 80 backend web >&2 || true
  set_env_value KNOX_API_VERSION "$previous_api"
  set_env_value KNOX_WEB_VERSION "$previous_web"
  export KNOX_API_VERSION="$previous_api" KNOX_WEB_VERSION="$previous_web"
  docker compose up -d --wait --wait-timeout 240 || true
  echo "Rolled back to api=$previous_api web=$previous_web. The pre-deploy database backup is in backups/." >&2
  exit 1
fi

docker image prune -f >/dev/null
log "Deployed api=$api web=$web"
docker compose ps
