FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY AppVecinos.Web.csproj .
RUN dotnet restore
COPY . .

RUN dotnet build "AppVecinos.Web.csproj" -c Release -o /app/build

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AppVecinos.Web.dll"]