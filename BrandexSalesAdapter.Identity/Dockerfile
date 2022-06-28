FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexSalesAdapter.Identity/BrandexSalesAdapter.Identity.csproj", "BrandexSalesAdapter.Identity/"]
COPY ["BrandexSalesAdapter/BrandexSalesAdapter.csproj", "BrandexSalesAdapter/"]
RUN dotnet restore "BrandexSalesAdapter.Identity/BrandexSalesAdapter.Identity.csproj"
COPY . .
WORKDIR "/src/BrandexSalesAdapter.Identity"
RUN dotnet build "BrandexSalesAdapter.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexSalesAdapter.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexSalesAdapter.Identity.dll"]
