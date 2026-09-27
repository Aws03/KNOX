# KNOX production deployment

This is the runbook for running KNOX on a single Linux host with Docker. Every command below runs in the deployment directory on the host (for example `/opt/knox`), which holds a copy of this folder.

```
browser ──HTTPS──▶ proxy (Caddy: TLS, HTTP/3, HSTS)
                     └──▶ web (nginx: SPA, /api proxy, /healthz)
                            └──▶ backend (API, read-only container, least-privilege SQL login)
                                   ├──▶ sqlserver (Express, internal network only, volume sqldata)
                                   └──▶ object storage (S3 API): knox-public + knox-private
browser ──presigned PUT / signed GET──▶ object storage or CDN (course files never touch the host)
```

| File | Purpose |
|---|---|
| `compose.yml` | The stack. Images come from GHCR (`ghcr.io/aws03/knox-api`, `ghcr.io/aws03/knox-web`). |
| `compose.storage.yml` | Optional self-hosted object storage (SeaweedFS), served through Caddy. |
| `compose.local.yml`, `.env.local.example` | Local development: the same stack built from source, over HTTP on localhost:8080 (see the main README). |
| `.env.example` | Every setting, with notes. Copy it to `.env` and `chmod 600` it. |
| `caddy/` | The Caddyfile and optional route snippets. |
| `scripts/deploy.sh` | Backup, then migrate, then a health-gated switch, with automatic rollback. |
| `scripts/backup.sh`, `scripts/restore.sh` | Database backups and restores. |
| `scripts/migrate-local-uploads.sh` | One-off import of files from the old local `/uploads` volume. |

## 1. Prerequisites

- A Linux host (x86-64; SQL Server has no arm64 image) with Docker Engine 24+ and the Compose plugin v2.20+, at least 4 GB RAM, and ports 80 and 443 open.
- A DNS `A`/`AAAA` record for your domain pointing at the host. Caddy obtains the certificate on first start.
- Object storage and a CDN (section 2). Self-hosting with `compose.storage.yml` also works for small installations.
- If the GHCR packages are private: `docker login ghcr.io` with a token that has `read:packages`.

## 2. Object storage and CDN (videos and files)

Create **two buckets**. Keep both private unless noted.

| Bucket | Holds | Read by browsers through |
|---|---|---|
| `knox-public` | Profile pictures and quiz images (small, permanent URLs) | `STORAGE_PUBLIC_BASE_URL`: a CDN pull zone on this bucket, or the bucket's public URL |
| `knox-private` | Course materials: documents and videos up to `MAX_MATERIAL_BYTES` (2 GB default) | Short-lived signed URLs only (`Storage:DownloadUrlMinutes`, 4 h default) |

### How uploads and delivery work

1. The browser asks `POST /api/files/material-uploads` for a presigned `PUT` URL, and then uploads the file directly to `knox-private` under `temp/materials/`.
2. `POST /api/courses/{id}/materials` verifies the object (it exists and is no larger than the limit) and copies it server-side to `materials/{courseId}/`. The copy gets `Cache-Control: max-age=31536000, immutable`.
3. Course listings return a signed URL per file. The URL is either:
   - a **Bunny CDN token-authenticated URL**, when `CDN_PRIVATE_BASE_URL` and `CDN_TOKEN_KEY` are set, so videos stream from the edge cache; or
   - an **S3 presigned URL** straight from the bucket, otherwise.

   Range requests work either way, so videos seek without downloading everything.
4. Abandoned uploads in `temp/` are removed after 48 hours. Deleting a material deletes its file.

### Provider settings

- **Cloudflare R2:** `STORAGE_SERVICE_URL=https://<account-id>.r2.cloudflarestorage.com`, `STORAGE_REGION=auto`. Use an R2 API token scoped to the two buckets (Object Read & Write).
- **AWS S3:** `STORAGE_SERVICE_URL=https://s3.<region>.amazonaws.com`, `STORAGE_REGION=<region>`. Use an IAM user or role limited to `s3:GetObject`, `s3:PutObject`, `s3:DeleteObject` and `s3:ListBucket` on the two buckets.

### Bucket CORS

Browsers upload to `knox-private` directly, so that bucket needs a CORS rule. Serving `knox-public` directly (without a CDN) needs the same rule for `GET`.

```json
[{ "AllowedOrigins": ["https://knox.example.com"], "AllowedMethods": ["PUT", "GET", "HEAD"],
   "AllowedHeaders": ["Content-Type"], "ExposeHeaders": ["ETag"], "MaxAgeSeconds": 3600 }]
```

### Bunny CDN (the project's CDN)

1. **Public images.** Create a pull zone whose origin is the `knox-public` bucket. For a private bucket, enable S3 origin authentication with a read-only key. Set `STORAGE_PUBLIC_BASE_URL=https://<zone>.b-cdn.net`.
2. **Private files and videos.** Create a second pull zone on `knox-private`, with origin authentication and **Token Authentication** enabled. Set `CDN_PRIVATE_BASE_URL=https://<zone>.b-cdn.net` and `CDN_TOKEN_KEY=<the zone's token key>`. Unsigned or expired requests get 403 at the edge. Bunny caches by path (ignoring the token), so every viewer hits the same cached video.
3. **Caching.** Keep "respect origin Cache-Control" on. Objects are immutable (unique keys), so edge and browser caches never need purging.
4. **CSP.** Put the zone and bucket hosts in `CSP_MEDIA_SOURCES`, for example `https://<public-zone>.b-cdn.net https://<private-zone>.b-cdn.net https://<account-id>.r2.cloudflarestorage.com`.

Any other CDN can serve the public bucket. For signed private delivery without Bunny, leave the `CDN_*` settings empty to use S3 presigned URLs.

## 3. First deployment

```bash
sudo mkdir -p /opt/knox && sudo chown "$USER" /opt/knox
# copy this folder to /opt/knox (the GitHub "Deploy" workflow does this with rsync)
cd /opt/knox
cp .env.example .env && chmod 600 .env
$EDITOR .env        # DOMAIN, ACME_EMAIL, DB_*, JWT_SECRET, STORAGE_*, SEED_ADMIN_*, image versions
./scripts/deploy.sh sha-<api-commit> sha-<web-commit>
```

The first run creates the database, applies all migrations, creates the `knox_app` login (read/write data only), creates the SuperAdmin from `SEED_ADMIN_EMAIL`/`SEED_ADMIN_PASSWORD`, and starts everything behind HTTPS.

After it finishes:
1. Sign in as the SuperAdmin and change the password.
2. Clear `SEED_ADMIN_PASSWORD` in `.env`.

## 4. Releasing and rolling back

CI publishes an image per commit on the default branch: `sha-<short-commit>` (immutable, deploy these), `latest`, and semver tags for `v*` git tags. Deploy from the GitHub **Deploy** workflow (manual, behind the `production` environment's reviewers), or on the host:

```bash
./scripts/deploy.sh sha-1a2b3c4 sha-5d6e7f8   # api version, web version (omit one to keep it)
```

`deploy.sh` does the following in order:

1. Pulls the images.
2. Starts SQL Server.
3. Takes a verified backup (`backups/*-pre-deploy-<version>.bak`).
4. Runs `migrate` while the old version keeps serving.
5. Switches containers and waits up to 4 minutes for every health check.
6. If anything fails to become healthy, puts the previous images back and exits non-zero.

**Migration policy.** Keep migrations backward-compatible with the previous release: add first, and remove or rename in a later release. Then rolling the app back never needs a database restore. If a release did ship a breaking migration and must be undone, restore its pre-deploy backup (section 5) and deploy the previous version.

## 5. Backups and recovery

- **Schedule.** Add a nightly cron entry: `15 2 * * * /opt/knox/scripts/backup.sh nightly >> /opt/knox/backups/backup.log 2>&1`
- **What a backup is.** Each backup is a full `BACKUP DATABASE ... WITH CHECKSUM` followed by `RESTORE VERIFYONLY`, copied to `backups/` (mode 600) and pruned after `BACKUP_RETENTION_DAYS`.
- **Off-site.** Set `BACKUP_S3_URI` (plus credentials, and an endpoint for R2 or B2) to copy every backup off the host. Use a bucket with versioning and a lifecycle rule. Without an off-site copy, losing the host means losing the data.
- **Restore.** `./scripts/restore.sh backups/<file>.bak`
  1. Takes a safety backup.
  2. Stops the app.
  3. Restores.
  4. Re-runs `migrate`, which re-links the app login and applies newer migrations.
  5. Starts the app.
- **Test restores regularly**, for example monthly on a staging host.
- **Object storage** is not in these backups. Use the provider's durability together with bucket versioning or replication. With `compose.storage.yml`, back up the `knox_storage-data` volume, or sync the buckets elsewhere with `aws s3 sync`.
- **Recovery targets.** RPO is the backup interval (24 h with a nightly job; run the job more often if you need less). RTO is minutes on the same host. On a new host: restore `.env` from your secret store, run `deploy.sh`, then `restore.sh` the latest off-site backup.

## 6. Monitoring and logs

- `https://<domain>/healthz` returns `200 Healthy` only when the API can reach both SQL Server and object storage. Point an external uptime monitor (UptimeRobot, Better Stack, Grafana Cloud and so on) at it, with alerts to email or chat. Containers also have Docker health checks; `docker compose ps` shows them.
- The API logs JSON to stdout with trace ids, and every error response carries the same `traceId`. Logs rotate at 5 × 10 MB per container.
  - To follow the API: `docker compose logs -f backend`
  - To ship logs, point a collector (Vector, Promtail, the Datadog agent) at the Docker json-file logs.
- Alert on failures in the backup cron log and on disk usage of the Docker volumes and `backups/`.

## 7. Security notes

- Only Caddy publishes ports. The database is on an internal network with no internet route.
- App containers run as non-root with a read-only filesystem, no Linux capabilities and `no-new-privileges`.
- The API holds only a read/write SQL login. `sa` is used only by the one-shot `migrate` job.
- **Secrets.**
  - Secrets live only in `.env` on the host (mode 600) and in GitHub environment secrets; nothing secret is in the images or the repository.
  - Rotate `JWT_SECRET` to sign everyone out.
  - Rotate `DB_APP_PASSWORD` and redeploy: `migrate` re-keys the login.
- `MSSQL_PID=Express` is free for production. The Developer edition is not licensed for production use.
- Swagger/OpenAPI is off in production (`OpenApi__Enabled=false`).

## 8. Moving files from the old local storage

Earlier versions stored files in the `knox-uploads` Docker volume. The `ObjectStorageMaterials` migration already points existing materials at `legacy/...` keys. Copy the files and rewrite the image URLs:

```bash
docker run --rm -v knox_knox-uploads:/src -v "$PWD/old-uploads:/dst" alpine cp -a /src/. /dst/
./scripts/migrate-local-uploads.sh ./old-uploads http://old-site-origin:5173
```

## 9. GitHub Actions setup

1. In **Settings > Environments**, create `production` and add required reviewers.
2. Add the environment's **secrets**:
   - `DEPLOY_SSH_KEY`: a key for a deploy user in the `docker` group.
   - `DEPLOY_KNOWN_HOSTS`: the output of `ssh-keyscan <host>`.
3. Add the environment's **variables**: `DEPLOY_HOST`, `DEPLOY_USER`, `DEPLOY_PATH`, `DOMAIN`.
4. In the package settings, grant the repository access to its GHCR packages if they are private.
