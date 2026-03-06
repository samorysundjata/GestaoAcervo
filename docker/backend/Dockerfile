# =============================================================================
# Stage 1: Build
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto para restaurar dependências (cache de layers)
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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Cria usuário não-root para segurança
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

# Copia apenas os artefatos publicados
COPY --from=build /app/publish .

# Porta exposta pela API
EXPOSE 8080

# Variável de ambiente padrão
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Acervo.API.dll"]