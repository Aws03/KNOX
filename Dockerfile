# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so restore is cached independently of source changes
COPY JadaraITKnowledgeSystem.sln .
COPY JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj JadaraITKnowledgeSystem.API/
COPY JadaraITKnowledgeSystem.Application/JadaraITKnowledgeSystem.Application.csproj JadaraITKnowledgeSystem.Application/
COPY JadaraITKnowledgeSystem.Domain/JadaraITKnowledgeSystem.Domain.csproj JadaraITKnowledgeSystem.Domain/
COPY JadaraITKnowledgeSystem.Infrastructure/JadaraITKnowledgeSystem.Infrastructure.csproj JadaraITKnowledgeSystem.Infrastructure/

RUN dotnet restore JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj

COPY . .

RUN dotnet publish JadaraITKnowledgeSystem.API/JadaraITKnowledgeSystem.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Program.cs skips the HTTPS (6001) listener when DOTNET_RUNNING_IN_CONTAINER=true,
# which the base image already sets, and always binds HTTP on 5001.
EXPOSE 5001

ENTRYPOINT ["dotnet", "JadaraITKnowledgeSystem.API.dll"]
