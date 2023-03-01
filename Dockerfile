FROM mcr.microsoft.com/dotnet/sdk:6.0 AS buildmigrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN ls

COPY . /build
WORKDIR /build/OpenttdDiscord.Database
RUN dotnet ef migrations script > /script.sql 

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
COPY ./OpenttdDiscord.Validation/OpenttdDiscord.Validation.csproj ./OpenttdDiscord.Validation/OpenttdDiscord.Validation.csproj
COPY ./OpenttdDiscord.Validation.Tests/OpenttdDiscord.Validation.Tests.csproj ./OpenttdDiscord.Validation.Tests/OpenttdDiscord.Validation.Tests.csproj
#END_PUT_PROJECTS_BELOW_THIS_LINE

COPY ./OpenttdDiscord.sln .
RUN dotnet restore 
COPY . .

FROM build AS publish
ARG CONFIGURATION=Release

RUN dotnet publish "OpenttdDiscord.Discord/OpenttdDiscord.Discord.csproj" -c $CONFIGURATION -o /app/publish
RUN dotnet publish "OpenttdDiscord.Database.Migrator/OpenttdDiscord.Database.Migrator.csproj" -c $CONFIGURATION -o /app/migrator

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS run
ARG CONFIGURATION=Release

WORKDIR /app

COPY --from=publish /app .
COPY --from=buildmigrations /script.sql /app/script.sql
COPY ./startup.sh .
ENTRYPOINT ["bash", "-c", "./startup.sh"]


