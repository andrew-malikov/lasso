FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Finance.WebApi/Finance.WebApi.csproj", "Finance.WebApi/"]
COPY ["Finance.Grpc/Finance.Grpc.csproj", "Finance.Grpc/"]
COPY ["Finance.Db/Finance.Db.csproj", "Finance.Db/"]
RUN dotnet restore "Finance.WebApi/Finance.WebApi.csproj"
COPY . .
WORKDIR "/src/Finance.WebApi"
RUN dotnet build "./Finance.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finance.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Finance.WebApi.dll"]
