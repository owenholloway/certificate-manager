FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

RUN ["apt-get", "update"]

WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0

WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "CertificateManager.dll"]