# =====================
# Build stage
# =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/API/API.csproj src/API/

RUN dotnet restore "src/API/API.csproj" /p:Configuration=Release

COPY . .
WORKDIR /src/src/API
RUN dotnet publish -c Release -o /app/publish --no-restore

# =====================
# Runtime stage
# =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

RUN apk add --no-cache icu-libs curl

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_GC_SERVER=1 \
    DOTNET_RUNNING_IN_CONTAINER=true

RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser

WORKDIR /app
COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]