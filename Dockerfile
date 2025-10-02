# ----------------------
# 1. Build stage
# ----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj và restore dependency
COPY ["QE180082_Ass1_Product/QE180082_Ass1_Product.csproj", "QE180082_Ass1_Product/"]
RUN dotnet restore "QE180082_Ass1_Product/QE180082_Ass1_Product.csproj"

# Copy toàn bộ source code và build
COPY . .
WORKDIR "/src/QE180082_Ass1_Product"
RUN dotnet publish "QE180082_Ass1_Product.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ----------------------
# 2. Runtime stage
# ----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose cổng cho API
EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "QE180082_Ass1_Product.dll"]
