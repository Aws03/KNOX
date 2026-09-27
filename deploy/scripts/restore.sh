#!/usr/bin/env bash
# Restores a backup made by backup.sh over the live database, then re-runs the migrate step
# (applies newer migrations and re-links the API's SQL login). The API is stopped meanwhile.
# Usage: scripts/restore.sh backups/<file>.bak [--yes]
source "$(dirname "$0")/common.sh"

backup="${1:?Usage: scripts/restore.sh backups/<file>.bak [--yes]}"
[[ -f "$backup" ]] || { echo "error: $backup not found" >&2; exit 1; }
db=$(env_value DB_NAME KnoxDb)

if [[ "${2:-}" != "--yes" ]]; then
  read -r -p "Replace database $db with $(basename "$backup")? Everything since that backup is lost. [y/N] " answer
  [[ "$answer" == [yY] ]] || exit 1
fi

log "Taking a safety backup of the current state"
"$DEPLOY_DIR/scripts/backup.sh" pre-restore

log "Stopping the API"
docker compose stop backend web

log "Restoring $db from $backup"
remote="/var/opt/mssql/backup/restore.bak"
docker compose exec -T sqlserver mkdir -p /var/opt/mssql/backup
docker compose cp "$backup" "sqlserver:$remote"
docker compose exec -T -u 0 sqlserver chown mssql "$remote"
sqlcmd -Q "IF DB_ID(N'$db') IS NOT NULL ALTER DATABASE [$db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
           RESTORE DATABASE [$db] FROM DISK = N'$remote' WITH REPLACE, CHECKSUM, STATS = 25;
           ALTER DATABASE [$db] SET MULTI_USER;"
docker compose exec -T sqlserver rm -f "$remote"

log "Running the migrate step"
docker compose run --rm migrate

log "Starting the API"
docker compose up -d --wait --wait-timeout 180 backend web
echo "Restore complete."
