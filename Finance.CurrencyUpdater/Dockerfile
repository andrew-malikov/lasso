FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Finance.CurrencyUpdater/Finance.CurrencyUpdater.csproj", "Finance.CurrencyUpdater/"]
COPY ["Finance.Db/Finance.Db.csproj", "Finance.Db/"]
RUN dotnet restore "Finance.CurrencyUpdater/Finance.CurrencyUpdater.csproj"
COPY . .
WORKDIR "/src/Finance.CurrencyUpdater"
RUN dotnet build "./Finance.CurrencyUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finance.CurrencyUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Finance.CurrencyUpdater.dll"]
