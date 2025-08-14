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

# Ports utilisés par l'app (HTTP/HTTPS)
EXPOSE 8080
EXPOSE 8443

# L'appli écoute sur HTTP et HTTPS (le compose mappe 8095->8080 et 8096->8443)
ENV ASPNETCORE_URLS="http://+:8080;https://+:8443"

# (Optionnel) Si tu fournis un certificat .pfx via un volume:
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx"
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password="tonMotDePassePfx"

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SaillingLoc.dll"]
