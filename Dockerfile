# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore (layer-cache friendly)
COPY QuantityMeasurementApp.Models/QuantityMeasurementApp.Models.csproj               QuantityMeasurementApp.Models/
COPY QuantityMeasurementApp.Repositories/QuantityMeasurementApp.Repositories.csproj   QuantityMeasurementApp.Repositories/
COPY QuantityMeasurementApp.Business/QuantityMeasurementApp.Business.csproj           QuantityMeasurementApp.Business/
COPY QuantityMeasurementApp.Controllers/QuantityMeasurementApp.Controllers.csproj     QuantityMeasurementApp.Controllers/
COPY QuantityMeasurementApp.API/QuantityMeasurementApp.API.csproj                     QuantityMeasurementApp.API/

RUN dotnet restore QuantityMeasurementApp.API/QuantityMeasurementApp.API.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish QuantityMeasurementApp.API/QuantityMeasurementApp.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT; default to 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

ENTRYPOINT ["dotnet", "QuantityMeasurementApp.API.dll"]
