FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Users.Db/Users.Db.csproj", "Users.Db/"]
RUN dotnet restore "Users.Db/Users.Db.csproj"
COPY [".config/dotnet-tools.json", ".config/dotnet-tools.json"]
RUN dotnet tool restore
COPY ["Users.Db", "Users.Db"]
ENV UsersDbContext=none
RUN dotnet ef migrations bundle --self-contained -r linux-x64 --project Users.Db -o ./bin/efbundle

FROM ubuntu:24.10 as migrator
RUN apt update -y && apt install -y libicu-dev 
WORKDIR /migrator
COPY --from=build /src/bin .
COPY --from=build /src/Users.Db/appsettings.json .
RUN chmod +x ./efbundle
CMD ["sh", "-c", "cd /migrator && ./efbundle --connection ${UsersDbContext}"]
