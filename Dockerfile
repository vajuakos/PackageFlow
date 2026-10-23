FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Recreates docker cache only if dependencies changed
COPY ["PackageFlow.API/PackageFlow.API.csproj", "PackageFlow.API/"]
COPY ["PackageFlow.Shared/PackageFlow.Shared.csproj", "PackageFlow.Shared/"]

RUN dotnet restore "PackageFlow.API/PackageFlow.API.csproj"

# Copy full source
COPY . .
RUN dotnet publish "PackageFlow.API/PackageFlow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Graphical dependencies
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "PackageFlow.API.dll"]