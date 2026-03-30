# Bu dosya, uygulamayı herhangi bir Linux/Windows sunucusunda çalışabilen "Gemi Konteynerına" koyar.
# Aşama 1: Sadece derleme işlemlerini yapan ağır işçi (SDK) ortamı
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AspNetCoreCrudDemo/AspNetCoreCrudDemo.csproj", "AspNetCoreCrudDemo/"]
RUN dotnet restore "AspNetCoreCrudDemo/AspNetCoreCrudDemo.csproj"
COPY . .
WORKDIR "/src/AspNetCoreCrudDemo"
RUN dotnet publish "AspNetCoreCrudDemo.csproj" -c Release -o /app/publish

# Aşama 2: Sadece çalıştırmayı yapan hafif ve temiz ortam (Müşteriye gidecek olan kısım)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreCrudDemo.dll"]
