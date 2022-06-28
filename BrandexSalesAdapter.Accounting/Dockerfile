FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexSalesAdapter.Accounting/BrandexSalesAdapter.Accounting.csproj", "BrandexSalesAdapter.Accounting/"]
COPY ["BrandexSalesAdapter/BrandexSalesAdapter.csproj", "BrandexSalesAdapter/"]
RUN dotnet restore "BrandexSalesAdapter.Accounting/BrandexSalesAdapter.Accounting.csproj"
COPY . .
WORKDIR "/src/BrandexSalesAdapter.Accounting"
RUN dotnet build "BrandexSalesAdapter.Accounting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexSalesAdapter.Accounting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexSalesAdapter.Accounting.dll"]
