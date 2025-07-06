# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY DiscordCloneBackend/*.csproj ./DiscordCloneBackend/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the files
COPY . .

# Publish
RUN dotnet publish DiscordCloneBackend/DiscordCloneBackend.csproj -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DiscordCloneBackend.dll"]
