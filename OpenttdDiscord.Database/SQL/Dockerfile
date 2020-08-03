FROM mysql:latest
# needed for intialization
ENV MYSQL_ROOT_PASSWORD=root
ENV MYSQL_DATABASE=openttd
ENV MYSQL_USER=openttd
ENV MYSQL_PASSWORD=secret-pw

COPY *.sql /docker-entrypoint-initdb.d/