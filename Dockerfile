FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore src/Kastle.DocMind.Api/Kastle.DocMind.Api.csproj

RUN dotnet publish src/Kastle.DocMind.Api/Kastle.DocMind.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Kastle.DocMind.Api.dll"]