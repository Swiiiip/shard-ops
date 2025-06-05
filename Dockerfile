FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shard.RayanCedric.API/Shard.RayanCedric.API.csproj", "Shard.RayanCedric.API/"]
COPY ["Shard.Shared.Core/Shard.Shared.Core.csproj", "Shard.Shared.Core/"]
COPY ["Shard.RayanCedric.IntegrationTests/Shard.RayanCedric.IntegrationTests.csproj", "Shard.RayanCedric.IntegrationTests/"]
COPY ["Shard.Shared.Web.IntegrationTests/Shard.Shared.Web.IntegrationTests.csproj", "Shard.Shared.Web.IntegrationTests/"]

RUN dotnet restore "Shard.RayanCedric.API/Shard.RayanCedric.API.csproj"

COPY . .

WORKDIR "/src/Shard.RayanCedric.API"
RUN dotnet build "Shard.RayanCedric.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shard.RayanCedric.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shard.RayanCedric.API.dll"]
