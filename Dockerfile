# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /source

# Install npm for publish step later (so we can build React)
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_22.x| bash - && apt-get install -yq nodejs build-essential

# Copy project file and restore as distinct layers
COPY --link ./MovieInfoBackend/MovieInfoBackend/*.csproj .
RUN dotnet restore -a $TARGETARCH

# Copy source code and publish app
FROM build AS publish
COPY --link MovieInfoBackend/MovieInfoBackend/. .
COPY --link movie-info-frontend movie-info-frontend
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS deploy
ENV ASPNETCORE_HTTP_PORT=8080
EXPOSE 8080
WORKDIR /app
COPY --link --from=publish /app .
USER $APP_UID
ENTRYPOINT ["./MovieInfoBackend"]
