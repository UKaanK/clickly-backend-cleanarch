# Clickly.Api/Dockerfile

# 1. Aşama: Uygulamayı build etme (SDK imajı)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Clickly.Api/Clickly.Api.csproj", "Clickly.Api/"]
COPY ["Clickly.Application/Clickly.Application.csproj", "Clickly.Application/"]
COPY ["Clickly.Domain/Clickly.Domain.csproj", "Clickly.Domain/"]
COPY ["Clickly.Infrastructure/Clickly.Infrastructure.csproj", "Clickly.Infrastructure/"]

# NuGet paketlerini geri yükle
RUN dotnet restore "Clickly.Api/Clickly.Api.csproj"

# Tüm projeleri kopyala
COPY . .
WORKDIR "/src/Clickly.Api"

# Uygulamayı Release modunda publish et
RUN dotnet publish "Clickly.Api.csproj" -c Release -o /app/publish

# 2. Aşama: Uygulamayı çalıştırma (minimalist ASP.NET Core imajı)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Çalıştırma komutunu tanımla
ENTRYPOINT ["dotnet", "Clickly.Api.dll"]