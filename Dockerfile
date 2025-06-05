# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443

COPY ["Shard.API/Shard.API.csproj", "Shard.API/"]
COPY ["Shard.Shared.Core/Shard.Shared.Core.csproj", "Shard.Shared.Core/"]
COPY ["Shard.Shared.Web.IntegrationTests/Shard.Shared.Web.IntegrationTests.csproj", "Shard.Shared.Web.IntegrationTests/"]

RUN dotnet restore "Shard.API/Shard.API.csproj"
COPY . .
WORKDIR "/src/Shard.API"
RUN dotnet build "Shard.API.csproj" -c Release -o /app/build
RUN dotnet publish "Shard.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Build-time argument to set version
ARG API_VERSION=dev
ENV API_VERSION=$API_VERSION

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Shard.API.dll"]
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443

COPY ["Shard.API/Shard.API.csproj", "Shard.API/"]
COPY ["Shard.Shared.Core/Shard.Shared.Core.csproj", "Shard.Shared.Core/"]
COPY ["Shard.Shared.Web.IntegrationTests/Shard.Shared.Web.IntegrationTests.csproj", "Shard.Shared.Web.IntegrationTests/"]

RUN dotnet restore "Shard.API/Shard.API.csproj"
COPY . .
WORKDIR "/src/Shard.API"
RUN dotnet build "Shard.API.csproj" -c Release -o /app/build
RUN dotnet publish "Shard.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Build-time argument to set version
ARG API_VERSION=dev
ENV API_VERSION=$API_VERSION

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Shard.API.dll"]