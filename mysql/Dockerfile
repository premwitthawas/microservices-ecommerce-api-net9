FROM alpine:latest AS builder
WORKDIR /app
COPY mysql-init ./
RUN apk add --no-cache openssl

FROM mysql:latest
COPY --from=builder /app /docker-entrypoint-initdb.d
ENV MYSQL_ROOT_PASSWORD=ec31d315916485460cadb7fc58c90118795ddeccb75147e5d38d4dbcde433c1c
ENV MYSQL_DATABASE=ecommercedbproduct
ENV MYSQL_PASSWORD=901a923055b3341e6effff67a8e133391c66aa2921dbedf1b834de0f583ec85b
EXPOSE 3306