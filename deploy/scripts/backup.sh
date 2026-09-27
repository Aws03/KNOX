#!/usr/bin/env bash
# Full, checksummed and verified SQL Server backup into deploy/backups/, pruned after
# BACKUP_RETENTION_DAYS, with an optional off-site copy (BACKUP_S3_URI).
# Usage: scripts/backup.sh [label]        e.g. from cron: 15 2 * * * /opt/knox/scripts/backup.sh nightly
source "$(dirname "$0")/common.sh"

label="${1:-manual}"
db=$(env_value DB_NAME KnoxDb)
retention=$(env_value BACKUP_RETENTION_DAYS 14)
file="${db}-$(date -u +%Y%m%dT%H%M%SZ)-${label}.bak"
remote="/var/opt/mssql/backup/$file"

if [[ "$(sqlcmd -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID(N'$db') IS NULL THEN 0 ELSE 1 END")" != "1" ]]; then
  echo "Database $db does not exist yet; nothing to back up."
  exit 0
fi

log "Backing up $db to $file"
docker compose exec -T sqlserver mkdir -p /var/opt/mssql/backup
sqlcmd -Q "BACKUP DATABASE [$db] TO DISK = N'$remote' WITH CHECKSUM, INIT, FORMAT, STATS = 25;
           RESTORE VERIFYONLY FROM DISK = N'$remote' WITH CHECKSUM;"

mkdir -p backups
chmod 700 backups
docker compose cp "sqlserver:$remote" "backups/$file"
docker compose exec -T sqlserver rm -f "$remote"
chmod 600 "backups/$file"
echo "Saved backups/$file ($(du -h "backups/$file" | cut -f1))"

find backups -name '*.bak' -type f -mtime +"$retention" -print -delete

s3_uri=$(env_value BACKUP_S3_URI)
if [[ -n "$s3_uri" ]]; then
  log "Copying off-site to $s3_uri"
  endpoint=$(env_value BACKUP_S3_ENDPOINT)
  docker run --rm -v "$DEPLOY_DIR/backups:/backups:ro" \
    -e AWS_ACCESS_KEY_ID="$(env_value BACKUP_S3_ACCESS_KEY)" \
    -e AWS_SECRET_ACCESS_KEY="$(env_value BACKUP_S3_SECRET_KEY)" \
    -e AWS_DEFAULT_REGION="$(env_value BACKUP_S3_REGION auto)" \
    amazon/aws-cli:2.37.4 s3 cp "/backups/$file" "$s3_uri/$file" ${endpoint:+--endpoint-url "$endpoint"}
fi
