FROM microsoft/aspnetcore-build:2.0.0 AS build

WORKDIR /code

COPY . .

WORKDIR /code/src/ShtikLive.Shows

RUN dotnet restore

RUN dotnet publish --output /app/ --configuration Release

FROM microsoft/aspnetcore:2.0.0

COPY --from=build /app /app/

WORKDIR /app

ENTRYPOINT ["dotnet", "ShtikLive.Shows.dll"]