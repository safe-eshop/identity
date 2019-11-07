FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100 AS build
WORKDIR /src
COPY . .
RUN dotnet tool install --global paket
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR /src/src/Identity.Api
RUN paket restore
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM build as app-test
WORKDIR /src/test/Identity.ApplicationTests
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet run

FROM build as domain-test
WORKDIR /src/test/Identity.DomainTests
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet run

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]