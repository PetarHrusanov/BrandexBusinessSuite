FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.MarketingAnalysis/BrandexBusinessSuite.MarketingAnalysis.csproj", "BrandexBusinessSuite.MarketingAnalysis/"]
COPY ["BrandexBusinessSuite/BrandexBusinessSuite.csproj", "BrandexBusinessSuite/"]
RUN dotnet restore "BrandexBusinessSuite.MarketingAnalysis/BrandexBusinessSuite.MarketingAnalysis.csproj"
COPY . .
WORKDIR "/src/BrandexBusinessSuite.MarketingAnalysis"
RUN dotnet build "BrandexBusinessSuite.MarketingAnalysis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.MarketingAnalysis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.MarketingAnalysis.dll"]
