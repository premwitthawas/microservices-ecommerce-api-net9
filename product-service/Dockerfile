FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /services
COPY Ecommerce.ProductService.sln .
COPY src ./src
RUN dotnet restore
WORKDIR /services/src/Ecommerce.Product.API
RUN dotnet build "./Ecommerce.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecommerce.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MYSQL_HOST=localhost
ENV MYSQL_PORT=3306
ENV MYSQL_USER=root
ENV MYSQL_PASSWORD=root
ENV MYSQL_DATABASE=ecommerce
ENV RABBITMQ_HOST=localhost
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_USER=guest,
ENV RABBITMQ_PASSWORD=guest
ENV RABBITMQ_EXCHANGE=products.exchange
ENTRYPOINT ["dotnet", "Ecommerce.Product.API.dll"]