FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore RotaCerta.API/RotaCerta.API.csproj

RUN dotnet publish RotaCerta.API/RotaCerta.API.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RotaCerta.API.dll"]