FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.Accounting/BrandexBusinessSuite.Accounting.csproj", "BrandexBusinessSuite.Accounting/"]
COPY ["BrandexBusinessSuite/BrandexBusinessSuite.csproj", "BrandexBusinessSuite/"]
RUN dotnet restore "BrandexBusinessSuite.Accounting/BrandexBusinessSuite.Accounting.csproj"
COPY . .
WORKDIR "/src/BrandexSalesAdapter.Accounting"
RUN dotnet build "BrandexBusinessSuite.Accounting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.Accounting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.Accounting.dll"]
