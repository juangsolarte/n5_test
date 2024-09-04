# Usar la imagen oficial de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos de proyecto y restaurar las dependencias
COPY *.sln .
COPY PermissionsApi/*.csproj ./PermissionsApi/
COPY PermissionsApi.Tests/*.csproj ./PermissionsApi.Tests/
RUN dotnet restore

# Copiar el resto de los archivos y construir la aplicación
COPY PermissionsApi/. ./PermissionsApi/
COPY PermissionsApi.Tests/. ./PermissionsApi.Tests/
WORKDIR /app/PermissionsApi
RUN dotnet publish -c Release -o out

# Usar la imagen oficial de .NET Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/PermissionsApi/out ./
ENTRYPOINT ["dotnet", "PermissionsApi.dll"]