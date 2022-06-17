ARG BASE_IMAGE=mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm64v8
FROM ${BASE_IMAGE} AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RaspiFanController/RaspiFanController.csproj", "RaspiFanController/"]
RUN dotnet restore "RaspiFanController/RaspiFanController.csproj"
COPY . .
WORKDIR "/src/RaspiFanController"
RUN dotnet build "RaspiFanController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RaspiFanController.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaspiFanController.dll"]
