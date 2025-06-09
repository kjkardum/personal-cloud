# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS dotnet-build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY CloudyBack/CloudyBack.slnx ./
COPY CloudyBack/Kjkardum.CloudyBack.Api/Kjkardum.CloudyBack.Api.csproj Kjkardum.CloudyBack.Api/
COPY CloudyBack/Kjkardum.CloudyBack.Application/Kjkardum.CloudyBack.Application.csproj Kjkardum.CloudyBack.Application/
COPY CloudyBack/Kjkardum.CloudyBack.Domain/Kjkardum.CloudyBack.Domain.csproj Kjkardum.CloudyBack.Domain/
COPY CloudyBack/Kjkardum.CloudyBack.Infrastructure/Kjkardum.CloudyBack.Infrastructure.csproj Kjkardum.CloudyBack.Infrastructure/
COPY CloudyBack/Tests/Kjkardum.CloudyBack.Api.Tests/Kjkardum.CloudyBack.Api.Tests.csproj Tests/Kjkardum.CloudyBack.Api.Tests/
COPY CloudyBack/Tests/Kjkardum.CloudyBack.Application.Tests/Kjkardum.CloudyBack.Application.Tests.csproj Tests/Kjkardum.CloudyBack.Application.Tests/
COPY CloudyBack/Tests/Kjkardum.CloudyBack.Infrastructure.Tests/Kjkardum.CloudyBack.Infrastructure.Tests.csproj Tests/Kjkardum.CloudyBack.Infrastructure.Tests/

RUN dotnet restore Kjkardum.CloudyBack.Api/Kjkardum.CloudyBack.Api.csproj

# Copy everything else and build
COPY CloudyBack .

RUN dotnet publish -c Release -o out

FROM node:22 AS node-env
WORKDIR /app
ENV YARN_VERSION=4.5.0
ENV NODE_ENV=development
RUN corepack enable && corepack prepare yarn@${YARN_VERSION}
COPY CloudyFront/ ./
RUN rm -rf node_modules
RUN yarn install
RUN yarn build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=dotnet-build-env /app/out .
COPY --from=node-env /app/dist ./wwwroot

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80
ENTRYPOINT ["dotnet", "Kjkardum.CloudyBack.Api.dll"]
