#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SolarEdge.Monitoring.Demo/SolarEdge.Monitoring.Demo.csproj", "src/SolarEdge.Monitoring.Demo/"]
RUN dotnet restore "src/SolarEdge.Monitoring.Demo/SolarEdge.Monitoring.Demo.csproj"
COPY . .
WORKDIR "/src/src/SolarEdge.Monitoring.Demo"
RUN dotnet build "SolarEdge.Monitoring.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SolarEdge.Monitoring.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolarEdge.Monitoring.Demo.dll"]