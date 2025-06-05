FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shard.API/Shard.API.csproj", "Shard.API/"]
COPY ["Shard.Shared.Core/Shard.Shared.Core.csproj", "Shard.Shared.Core/"]
COPY ["Shard.Shared.Web.IntegrationTests/Shard.Shared.Web.IntegrationTests.csproj", "Shard.Shared.Web.IntegrationTests/"]

RUN dotnet restore "Shard.API/Shard.API.csproj"

COPY . .

WORKDIR "/src/Shard.API"
RUN dotnet build "Shard.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shard.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shard.API.dll"]
