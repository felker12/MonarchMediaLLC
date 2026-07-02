FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files first (best layer caching)
COPY MonarchMediaLLC.ApiService/MonarchMediaLLC.ApiService.csproj MonarchMediaLLC.ApiService/
COPY MonarchMediaLLC.Shared/MonarchMediaLLC.Shared.csproj MonarchMediaLLC.Shared/
COPY MonarchMediaLLC.ServiceDefaults/MonarchMediaLLC.ServiceDefaults.csproj MonarchMediaLLC.ServiceDefaults/

# Restore dependencies
WORKDIR /src/MonarchMediaLLC.ApiService
RUN dotnet restore

# Copy the remaining source code
WORKDIR /src
COPY . .

# Publish
WORKDIR /src/MonarchMediaLLC.ApiService
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MonarchMediaLLC.ApiService.dll"]