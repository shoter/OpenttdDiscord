ARG BUILD_IMG=mcr.microsoft.com/dotnet/sdk:7.0
FROM ${BUILD_IMG} AS build
ARG CONFIGURATION=Release

#PUT_PROJECTS_BELOW_THIS_LINE
COPY ./OpenttdDiscord.Base/OpenttdDiscord.Base.csproj ./OpenttdDiscord.Base/OpenttdDiscord.Base.csproj
COPY ./OpenttdDiscord.Database/OpenttdDiscord.Database.csproj ./OpenttdDiscord.Database/OpenttdDiscord.Database.csproj
COPY ./OpenttdDiscord.Database.Migrator/OpenttdDiscord.Database.Migrator.csproj ./OpenttdDiscord.Database.Migrator/OpenttdDiscord.Database.Migrator.csproj
COPY ./OpenttdDiscord.Database.Tests/OpenttdDiscord.Database.Tests.csproj ./OpenttdDiscord.Database.Tests/OpenttdDiscord.Database.Tests.csproj
COPY ./OpenttdDiscord.Discord/OpenttdDiscord.Discord.csproj ./OpenttdDiscord.Discord/OpenttdDiscord.Discord.csproj
COPY ./OpenttdDiscord.DockerizedTesting/OpenttdDiscord.DockerizedTesting.csproj ./OpenttdDiscord.DockerizedTesting/OpenttdDiscord.DockerizedTesting.csproj
COPY ./OpenttdDiscord.Domain/OpenttdDiscord.Domain.csproj ./OpenttdDiscord.Domain/OpenttdDiscord.Domain.csproj
COPY ./OpenttdDiscord.Infrastructure/OpenttdDiscord.Infrastructure.csproj ./OpenttdDiscord.Infrastructure/OpenttdDiscord.Infrastructure.csproj
COPY ./OpenttdDiscord.Infrastructure.Tests/OpenttdDiscord.Infrastructure.Tests.csproj ./OpenttdDiscord.Infrastructure.Tests/OpenttdDiscord.Infrastructure.Tests.csproj
COPY ./OpenttdDiscord.Tests.Common/OpenttdDiscord.Tests.Common.csproj ./OpenttdDiscord.Tests.Common/OpenttdDiscord.Tests.Common.csproj
COPY ./OpenttdDiscord.Validation/OpenttdDiscord.Validation.csproj ./OpenttdDiscord.Validation/OpenttdDiscord.Validation.csproj
COPY ./OpenttdDiscord.Validation.Tests/OpenttdDiscord.Validation.Tests.csproj ./OpenttdDiscord.Validation.Tests/OpenttdDiscord.Validation.Tests.csproj
#END_PUT_PROJECTS_BELOW_THIS_LINE

COPY ./OpenttdDiscord.sln .
RUN dotnet restore --disable-parallel
COPY . /build

FROM build AS publish
ARG CONFIGURATION=Release

RUN dotnet publish "/build/OpenttdDiscord.Discord/OpenttdDiscord.Discord.csproj" -c $CONFIGURATION -o /app/publish
RUN dotnet publish "/build/OpenttdDiscord.Database.Migrator/OpenttdDiscord.Database.Migrator.csproj" -c $CONFIGURATION -o /app/migrator

FROM build as dbMigrations

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /build/OpenttdDiscord.Database
RUN dotnet ef migrations script -v -i -o /script.sql 

ARG RUN_IMG=mcr.microsoft.com/dotnet/aspnet:6.0
FROM ${BUILD_IMG} AS run
ARG CONFIGURATION=Release

WORKDIR /app

COPY --from=publish /app .
COPY --from=dbMigrations /script.sql /app/script.sql
COPY ./startup.sh .
RUN chmod a+x /app/startup.sh
RUN mkdir -p /var/app/ottd/
ENTRYPOINT ["bash", "-c", "./startup.sh"]




