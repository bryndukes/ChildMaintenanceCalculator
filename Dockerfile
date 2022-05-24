#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=https://0.0.0.0:5000

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["ChildMaintenanceCalculator/ChildMaintenanceCalculator.csproj", "ChildMaintenanceCalculator/"]
RUN dotnet restore "ChildMaintenanceCalculator/ChildMaintenanceCalculator.csproj"
COPY . .
WORKDIR "/src/ChildMaintenanceCalculator"
RUN dotnet build "ChildMaintenanceCalculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChildMaintenanceCalculator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChildMaintenanceCalculator.dll"]