# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los csproj y restaurar dependencias
COPY Chat.Api/Chat.Api.csproj Chat.Api/
COPY Chat.Domain/Chat.Domain.csproj Chat.Domain/
COPY Chat.Shared/Chat.Shared.csproj Chat.Shared/
COPY Chat.Data/Chat.Data.csproj Chat.Data/
COPY Chat.Services/Chat.Services.csproj Chat.Services/
RUN dotnet restore Chat.Api/Chat.Api.csproj

# Copiar todo el código y compilar
COPY . .
WORKDIR /src/Chat.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Chat.Api.dll"]
