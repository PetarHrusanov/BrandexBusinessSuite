FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.SalesBrandex/BrandexBusinessSuite.SalesBrandex.csproj", "BrandexBusinessSuite.SalesBrandex/"]
RUN dotnet restore "BrandexBusinessSuite.SalesBrandex/BrandexBusinessSuite.SalesBrandex.csproj"
COPY . .
WORKDIR "/src/BrandexBusinessSuite.SalesBrandex"
RUN dotnet build "BrandexBusinessSuite.SalesBrandex.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.SalesBrandex.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.SalesBrandex.dll"]
