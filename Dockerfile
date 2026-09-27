# syntax=docker/dockerfile:1

# ---- Build ----
# The SDK runs natively on the build machine and cross-compiles for the target architecture,
# so multi-arch images (linux/amd64, linux/arm64) build without emulation.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src

# Restore first, from project files only, so the layer is reused until dependencies change.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY JadaraITKnowledgeSystem.Domain/JadaraITKnowledgeSystem.Domain.csproj JadaraITKnowledgeSystem.Domain/
COPY JadaraITKnowledgeSystem.Application/JadaraITKnowledgeSystem.Application.csproj JadaraITKnowledgeSystem.Application/
COPY JadaraITKnowledgeSystem.Infrastructure/JadaraITKnowledgeSystem.Infrastructure.csproj JadaraITKnowledgeSystem.Infrastructure/
COPY JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj JadaraITKnowledgeSystem.API/
RUN dotnet restore JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj -a $TARGETARCH

COPY JadaraITKnowledgeSystem.Domain/ JadaraITKnowledgeSystem.Domain/
COPY JadaraITKnowledgeSystem.Application/ JadaraITKnowledgeSystem.Application/
COPY JadaraITKnowledgeSystem.Infrastructure/ JadaraITKnowledgeSystem.Infrastructure/
COPY JadaraITKnowledgeSystem.API/ JadaraITKnowledgeSystem.API/
RUN dotnet publish JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj \
      -c Release -a $TARGETARCH -o /app/publish --no-restore -p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
ARG VERSION=dev
ARG REVISION=unknown
LABEL org.opencontainers.image.title="knox-api" \
      org.opencontainers.image.source="https://github.com/Aws03/KNOX" \
      org.opencontainers.image.version=$VERSION \
      org.opencontainers.image.revision=$REVISION
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=5001 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_gcServer=0

# Files live in object storage, so the app needs no writable directories (the root filesystem can be read-only).
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 5001

# Readiness: the API is up and can reach the database (no curl in this image; bash /dev/tcp instead).
HEALTHCHECK --interval=10s --timeout=5s --start-period=40s --retries=5 \
  CMD bash -c 'exec 3<>/dev/tcp/127.0.0.1/5001 && printf "GET /health/ready HTTP/1.0\r\nHost: localhost\r\n\r\n" >&3 && head -n1 <&3 | grep -q " 200 "'

# "migrate" as the command runs the deployment step (migrations, SQL login, seed) and exits.
ENTRYPOINT ["dotnet", "JadaraITKnowledgeSystem.API.dll"]
