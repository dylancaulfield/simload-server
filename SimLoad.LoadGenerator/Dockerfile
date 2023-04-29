FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimLoad.LoadGenerator/SimLoad.LoadGenerator.csproj", "SimLoad.LoadGenerator/"]
RUN dotnet restore "SimLoad.LoadGenerator/SimLoad.LoadGenerator.csproj"
COPY . .
WORKDIR "/src/SimLoad.LoadGenerator"
RUN dotnet build "SimLoad.LoadGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimLoad.LoadGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimLoad.LoadGenerator.dll"]
