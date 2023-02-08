FROM postgres:15.1 AS build
# needed for intialization
ENV MYSQL_ROOT_PASSWORD=root
ENV MYSQL_DATABASE=openttd
ENV MYSQL_USER=openttd
ENV MYSQL_PASSWORD=secret-pw

COPY ./sql/*.sql /docker-entrypoint-initdb.d/

# initialize database

FROM postgres:15.1 AS run
