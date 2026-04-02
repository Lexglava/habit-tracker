FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PortfolioProject.Api/PortfolioProject.Api.csproj", "PortfolioProject.Api/"]
RUN dotnet restore "PortfolioProject.Api/PortfolioProject.Api.csproj"

COPY . .
WORKDIR "/src/PortfolioProject.Api"
RUN dotnet publish "PortfolioProject.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PortfolioProject.Api.dll"]
