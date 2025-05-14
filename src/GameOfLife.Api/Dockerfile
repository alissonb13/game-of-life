FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GameOfLife.Api/GameOfLife.Api.csproj", "GameOfLife.Api/"]
COPY ["GameOfLife.Business/GameOfLife.Business.csproj", "GameOfLife.Business/"]
COPY ["GameOfLife.Infrastructure/GameOfLife.Infrastructure.csproj", "GameOfLife.Infrastructure/"]
RUN dotnet restore "GameOfLife.Api/GameOfLife.Api.csproj"
COPY . .
WORKDIR "/src/GameOfLife.Api"
RUN dotnet build "./GameOfLife.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GameOfLife.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY run_migration.sh .
RUN chmod +x run_migration.sh

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameOfLife.Api.dll"]
