# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY CloudyBack.slnx ./
COPY Kjkardum.CloudyBack.Api/Kjkardum.CloudyBack.Api.csproj Kjkardum.CloudyBack.Api/
COPY Kjkardum.CloudyBack.Application/Kjkardum.CloudyBack.Application.csproj Kjkardum.CloudyBack.Application/
COPY Kjkardum.CloudyBack.Domain/Kjkardum.CloudyBack.Domain.csproj Kjkardum.CloudyBack.Domain/
COPY Kjkardum.CloudyBack.Infrastructure/Kjkardum.CloudyBack.Infrastructure.csproj Kjkardum.CloudyBack.Infrastructure/
COPY Tests/Kjkardum.CloudyBack.Api.Tests/Kjkardum.CloudyBack.Api.Tests.csproj Tests/Kjkardum.CloudyBack.Api.Tests/
COPY Tests/Kjkardum.CloudyBack.Application.Tests/Kjkardum.CloudyBack.Application.Tests.csproj Tests/Kjkardum.CloudyBack.Application.Tests/
COPY Tests/Kjkardum.CloudyBack.Infrastructure.Tests/Kjkardum.CloudyBack.Infrastructure.Tests.csproj Tests/Kjkardum.CloudyBack.Infrastructure.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . .

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80
ENTRYPOINT ["dotnet", "Kjkardum.CloudyBack.Api.dll"]
