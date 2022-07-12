FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.OnlineShop/BrandexBusinessSuite.OnlineShop.csproj", "BrandexBusinessSuite.OnlineShop/"]
RUN dotnet restore "BrandexBusinessSuite.OnlineShop/BrandexBusinessSuite.OnlineShop.csproj"
COPY . .
WORKDIR "/src/BrandexBusinessSuite.OnlineShop"
RUN dotnet build "BrandexBusinessSuite.OnlineShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.OnlineShop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.OnlineShop.dll"]
