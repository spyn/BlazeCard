FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BlazeCard.sln .
COPY BlazeCard/BlazeCard.csproj BlazeCard/
RUN dotnet restore BlazeCard/BlazeCard.csproj

COPY BlazeCard/ BlazeCard/
RUN dotnet publish BlazeCard/BlazeCard.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/dp-keys && chmod 777 /app/dp-keys

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DataProtection__KeysPath=/app/dp-keys
EXPOSE 8080

ENTRYPOINT ["dotnet", "BlazeCard.dll"]
