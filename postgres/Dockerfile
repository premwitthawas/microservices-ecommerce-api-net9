FROM alpine:latest AS builder
WORKDIR /app
COPY postgres-init ./
RUN apk add --no-cache openssl

FROM postgres:latest
COPY --from=builder /app /docker-entrypoint-initdb.d
ENV POSTGRES_DB=ecommercedbauth
ENV POSTGRES_USER=ecommercedbauth
ENV POSTGRES_PASSWORD=52e91258b667c2b01de5
EXPOSE 5432