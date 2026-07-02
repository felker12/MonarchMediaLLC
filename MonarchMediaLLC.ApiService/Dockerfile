FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY MonarchMediaLLC.ApiService ./MonarchMediaLLC.ApiService

WORKDIR /src/MonarchMediaLLC.ApiService

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MonarchMediaLLC.ApiService.dll"]