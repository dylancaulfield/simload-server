FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

ARG CONFIG

WORKDIR /app

COPY . .

RUN dotnet restore "SimLoad.Server/SimLoad.Server.csproj"
RUN dotnet publish "SimLoad.Server/SimLoad.Server.csproj" -c $CONFIG -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0.13-alpine3.17-arm64v8 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "SimLoad.Server.dll"]
