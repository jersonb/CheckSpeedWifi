#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
RUN apt update && apt upgrade -y
RUN apt install speedtest-cli iputils-ping -y
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CheckSpeedWifi.WorkerService/CheckSpeedWifi.WorkerService.csproj", "CheckSpeedWifi.WorkerService/"]
RUN dotnet restore "./CheckSpeedWifi.WorkerService/./CheckSpeedWifi.WorkerService.csproj"
COPY . .
WORKDIR "/src/CheckSpeedWifi.WorkerService"
RUN dotnet build "./CheckSpeedWifi.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CheckSpeedWifi.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CheckSpeedWifi.WorkerService.dll"]