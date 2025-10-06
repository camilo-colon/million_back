FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["million.api/million.api.csproj", "million.api/"]
COPY ["million.application/million.application.csproj", "million.application/"]
COPY ["million.domain/million.domain.csproj", "million.domain/"]
COPY ["million.infrastructure/million.infrastructure.csproj", "million.infrastructure/"]

# Restore dependencies
RUN dotnet restore "million.api/million.api.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/million.api"
RUN dotnet build "million.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "million.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "million.api.dll"]
