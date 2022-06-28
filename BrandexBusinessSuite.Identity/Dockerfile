FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrandexBusinessSuite.Identity/BrandexBusinessSuite.Identity.csproj", "BrandexBusinessSuite.Identity/"]
COPY ["BrandexBusinessSuite/BrandexBusinessSuite.csproj", "BrandexBusinessSuite/"]
RUN dotnet restore "BrandexBusinessSuite.Identity/BrandexBusinessSuite.Identity.csproj"
COPY . .
WORKDIR "/src/BrandexBusinessSuite.Identity"
RUN dotnet build "BrandexBusinessSuite.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrandexBusinessSuite.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrandexBusinessSuite.Identity.dll"]
