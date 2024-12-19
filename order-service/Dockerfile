FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /services
COPY EcommerceOrderService.sln .
COPY src ./src
RUN dotnet restore
WORKDIR /services/src/Ecommerce.OrderService.API
RUN dotnet build "./Ecommerce.OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecommerce.OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV MONGO_USER=root
ENV MONGO_PASSWORD=root
ENV MONGO_HOST=localhost
ENV MONGO_PORT=27017
ENV MONGO_DATABASE=ecommerceorderservicedb
ENV USERS_MICROSERVICE_HOST=localhost
ENV USERS_MICROSERVICE_PORT=4000
ENV PRODUCTS_MICROSERVICE_HOST=localhost
ENV PRODUCTS_MICROSERVICE_PORT=4001
ENV REDIS_HOST=localhost
ENV REDIS_PORT=6379
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.OrderService.API.dll"]