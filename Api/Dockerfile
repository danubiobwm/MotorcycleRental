# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY *.sln .
COPY Domain/*.csproj Domain/
COPY Application/*.csproj Application/
COPY Infra/*.csproj Infra/
COPY Api/*.csproj Api/

RUN dotnet restore Api/Api.csproj

# Copiar o restante do código e publicar
COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]
