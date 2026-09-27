#!/usr/bin/env bash
# One-off migration from the old local /uploads storage (before object storage) into the buckets.
# Materials go to the private bucket under legacy/ (the ObjectStorageMaterials migration already points
# them there); images go to the public bucket under legacy/ and their stored URLs are rewritten.
# Usage: scripts/migrate-local-uploads.sh <old-uploads-dir> <old-site-origin>
#   e.g. docker run --rm -v knox_knox-uploads:/src -v "$PWD/old-uploads:/dst" alpine cp -a /src/. /dst/
#        scripts/migrate-local-uploads.sh ./old-uploads http://localhost:5173
source "$(dirname "$0")/common.sh"

uploads="$(cd "${1:?Usage: $0 <old-uploads-dir> <old-site-origin>}" && pwd)"
old_origin="${2:?Usage: $0 <old-uploads-dir> <old-site-origin>}"
old_origin="${old_origin%/}"
db=$(env_value DB_NAME KnoxDb)
public_bucket=$(env_value STORAGE_PUBLIC_BUCKET knox-public)
private_bucket=$(env_value STORAGE_PRIVATE_BUCKET knox-private)
public_base=$(env_value STORAGE_PUBLIC_BASE_URL)
public_base="${public_base%/}"

# The AWS CLI runs on the stack's network, so an internal endpoint (http://storage:8333) works too.
s3() {
  # Compose names the network <project>_app ("knox" unless COMPOSE_PROJECT_NAME overrides it).
  docker run --rm --network "$(env_value COMPOSE_PROJECT_NAME knox)_app" \
    -v "$uploads:/uploads:ro" \
    -e AWS_ACCESS_KEY_ID="$(env_value STORAGE_ACCESS_KEY)" \
    -e AWS_SECRET_ACCESS_KEY="$(env_value STORAGE_SECRET_KEY)" \
    -e AWS_DEFAULT_REGION="$(env_value STORAGE_REGION us-east-1)" \
    amazon/aws-cli:2.37.4 --endpoint-url "$(env_value STORAGE_SERVICE_URL)" "$@"
}

log "Copying course materials to s3://$private_bucket/legacy/"
if [[ -d "$uploads/permanent/material" ]]; then
  s3 s3 cp --recursive --only-show-errors /uploads/permanent/material "s3://$private_bucket/legacy/permanent/material"
fi

log "Copying images to s3://$public_bucket/legacy/"
s3 s3 cp --recursive --only-show-errors /uploads "s3://$public_bucket/legacy" --exclude "permanent/material/*" --exclude "temp/*"

log "Rewriting image URLs ($old_origin/uploads/ -> $public_base/legacy/)"
old_prefix="${old_origin//\'/\'\'}/uploads/"
new_prefix="${public_base//\'/\'\'}/legacy/"
sqlcmd -d "$db" -Q "
  UPDATE Users     SET ProfilePictureUrl = REPLACE(ProfilePictureUrl, N'$old_prefix', N'$new_prefix') WHERE ProfilePictureUrl LIKE N'$old_prefix%';
  UPDATE Questions SET ImageUrl = REPLACE(ImageUrl, N'$old_prefix', N'$new_prefix') WHERE ImageUrl LIKE N'$old_prefix%';
  UPDATE Choices   SET ImageUrl = REPLACE(ImageUrl, N'$old_prefix', N'$new_prefix') WHERE ImageUrl LIKE N'$old_prefix%';"
echo "Done. Check a few materials and images, then delete the old uploads volume."
