FROM mcr.microsoft.com/dotnet/sdk:6.0 AS buildmigrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN ls

COPY . /build
WORKDIR /build/OpenttdDiscord.Database
RUN dotnet ef migrations bundle 

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG CONFIGURATION=Release

#PUT_PROJECTS_BELOW_THIS_LINE
COPY ./OpenttdDiscord.Base/OpenttdDiscord.Base.csproj ./OpenttdDiscord.Base/OpenttdDiscord.Base.csproj
COPY ./OpenttdDiscord.Database/OpenttdDiscord.Database.csproj ./OpenttdDiscord.Database/OpenttdDiscord.Database.csproj
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

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS run
ARG CONFIGURATION=Release

WORKDIR /app

COPY --from=publish /app .
COPY ./startup.sh .

COPY --from=buildmigrations /build/OpenttdDiscord.Database/efbundle /bundle/efbundle
ENTRYPOINT ["bash", "-c", "./startup.sh"]

