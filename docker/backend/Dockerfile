# =============================================================================
# Stage 1: Build
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Do NOT copy global.json here. It pins a host SDK (e.g. 10.0.401) that often
# has no matching MCR tag; restore would fail with "SDK specified in
# global.json was not found". The image tag supplies the SDK. global.json is
# also listed in the build-context .dockerignore so `COPY . .` cannot sneak it
# back in.
COPY ["Acervo.API/Acervo.API.csproj",             "Acervo.API/"]
COPY ["Acervo.Application/Acervo.Application.csproj", "Acervo.Application/"]
COPY ["Acervo.Infrastructure/Acervo.Infrastructure.csproj", "Acervo.Infrastructure/"]
COPY ["Acervo.Domain/Acervo.Domain.csproj",       "Acervo.Domain/"]

RUN dotnet restore "Acervo.API/Acervo.API.csproj"

# Copia o restante do código-fonte
COPY . .

# Publica a aplicação em modo Release
WORKDIR /src/Acervo.API
RUN dotnet publish "Acervo.API.csproj" \
    -c Release \
    -o /app/publish

# =============================================================================
# Stage 2: Runtime
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user. The Ubuntu 24.04 aspnet image has groupadd/useradd, not addgroup/adduser.
RUN groupadd --system appgroup \
    && useradd --system --gid appgroup --home-dir /app --no-create-home appuser
USER appuser

# Copia apenas os artefatos publicados
COPY --from=build /app/publish .

# Porta exposta pela API
EXPOSE 8080

# Variável de ambiente padrão
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Acervo.API.dll"]