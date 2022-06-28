FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexSalesAdapter.ExcelLogic/BrandexSalesAdapter.ExcelLogic.csproj", "BrandexSalesAdapter.ExcelLogic/"]
COPY ["BrandexSalesAdapter/BrandexSalesAdapter.csproj", "BrandexSalesAdapter/"]
RUN dotnet restore "BrandexSalesAdapter.ExcelLogic/BrandexSalesAdapter.ExcelLogic.csproj"
COPY . .
WORKDIR "/src/BrandexSalesAdapter.ExcelLogic"
RUN dotnet build "BrandexSalesAdapter.ExcelLogic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexSalesAdapter.ExcelLogic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexSalesAdapter.ExcelLogic.dll"]
