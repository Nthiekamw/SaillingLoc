# STAGE 1: build + publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier uniquement le csproj d'abord pour profiter du cache
COPY SaillingLoc.csproj ./
RUN dotnet restore ./SaillingLoc.csproj

# Copier le reste du code
COPY . .
RUN dotnet publish ./SaillingLoc.csproj -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Ports par défaut des images .NET
EXPOSE 8080
EXPOSE 8443

# URL HTTP par défaut
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SaillingLoc.dll"]
