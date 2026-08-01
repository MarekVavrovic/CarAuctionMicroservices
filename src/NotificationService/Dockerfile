FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Carsties.sln ./
COPY src/NotificationService/NotificationService.csproj ./src/NotificationService/
COPY src/Contracts/Contracts.csproj ./src/Contracts/

# Restore package deps
RUN dotnet restore ./src/NotificationService/NotificationService.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish ./src/NotificationService/NotificationService.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]