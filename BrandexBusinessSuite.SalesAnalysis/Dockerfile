FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.SalesAnalysis/BrandexBusinessSuite.SalesAnalysis.csproj", "BrandexBusinessSuite.SalesAnalysis/"]
RUN dotnet restore "BrandexBusinessSuite.SalesAnalysis/BrandexBusinessSuite.SalesAnalysis.csproj"
COPY . .
WORKDIR "/src/BrandexBusinessSuite.SalesAnalysis"
RUN dotnet build "BrandexBusinessSuite.SalesAnalysis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.SalesAnalysis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.SalesAnalysis.dll"]
