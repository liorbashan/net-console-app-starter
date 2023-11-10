FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY CornerStone/ ./
RUN dotnet restore
RUN dotnet build --no-restore --configuration Release
RUN dotnet publish --no-build --configuration Release --output ./publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "CornerStone.dll"]
