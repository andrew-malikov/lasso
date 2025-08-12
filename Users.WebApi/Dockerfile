FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Users.WebApi/Users.WebApi.csproj", "Users.WebApi/"]
COPY ["Users.Application/Users.Application.csproj", "Users.Application/"]
COPY ["Users.Db/Users.Db.csproj", "Users.Db/"]
COPY ["Users.Grpc/Users.Grpc.csproj", "Users.Grpc/"]
RUN dotnet restore "Users.WebApi/Users.WebApi.csproj"
COPY . .
WORKDIR "/src/Users.WebApi"
RUN dotnet build "./Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.WebApi.dll"]
