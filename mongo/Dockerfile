FROM alpine:latest AS builder
WORKDIR /app
COPY mongo-init ./
RUN apk add --no-cache openssl

FROM mongo:latest
COPY --from=builder /app /docker-entrypoint-initdb.d

ENV MONGO_INITDB_DATABASE=ecommerceorderservicedb
ENV MONGO_INITDB_ROOT_USERNAME=root
ENV MONGO_INITDB_ROOT_PASSWORD=example

EXPOSE 27017
