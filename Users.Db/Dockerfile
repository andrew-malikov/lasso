FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Users.Db/Users.Db.csproj", "Users.Db/"]
RUN dotnet restore "Users.Db/Users.Db.csproj"
COPY [".config/dotnet-tools.json", ".config/dotnet-tools.json"]
RUN dotnet tool restore
COPY ["Users.Db", "Users.Db"]
RUN dotnet ef migrations bundle --self-contained -r linux-x64 --project Users.Db

FROM alpine:3.22 as migrator
COPY --from=build /src/efbundle .
ENTRYPOINT ["efbundle", "--connection", "${UsersDbContext}"]