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
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.OrderService.API.dll"]