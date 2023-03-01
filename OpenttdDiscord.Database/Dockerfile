FROM mcr.microsoft.com/dotnet/sdk:6.0 AS buildmigrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN ls

COPY . /build
WORKDIR /build/OpenttdDiscord.Database
RUN dotnet ef migrations script > /migration.sql

FROM postgres:15.1 AS db
# needed for intialization
ENV MYSQL_ROOT_PASSWORD=root
ENV POSTGRES_DB=openttd
ENV POSTGRES_USER=openttd
ENV POSTGRES_PASSWORD=secret-pw

COPY --from=buildmigrations /migration.sql .