﻿# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
WORKDIR OpenttdDiscord
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
# FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm32v7
WORKDIR /app
COPY --from=build /app .
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh /wait
RUN chmod +x /wait

COPY ./container_entry.sh /start.sh
RUN chmod +x /start.sh

ENTRYPOINT ["bash", "/start.sh"]