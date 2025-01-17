ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /app

# Restore DotNet Tools
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore

# Restore NuGet Packages
ARG APP_NAME=RichillCapital.TraderStudio.Web

COPY ./build.cake ./${APP_NAME}.sln ./${APP_NAME}.csproj ./
COPY ./Libs/RichillCapital.Domain/*.csproj ./Libs/RichillCapital.Domain/
COPY ./Libs/RichillCapital.Infrastructure/*.csproj ./Libs/RichillCapital.Infrastructure/
COPY ./Libs/RichillCapital.UseCases/*.csproj ./Libs/RichillCapital.UseCases/

RUN dotnet cake --target restore 

# Build and Publish Source Code
COPY . ./
RUN dotnet cake --target build && dotnet cake --target publish


FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

COPY --from=build ./app/publish ./

EXPOSE 8080
ENTRYPOINT [ "dotnet", "RichillCapital.TraderStudio.Web.dll" ]