FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CryptoAlertSystem.sln", "./"]
COPY ["src/services/CryptoAlert.PriceCollector/CryptoAlert.PriceCollector.csproj", "src/services/CryptoAlert.PriceCollector/"]
COPY ["src/shared/CryptoAlert.Contracts/CryptoAlert.Contracts.csproj", "src/shared/CryptoAlert.Contracts/"]
COPY ["src/shared/CryptoAlert.SharedKernel/CryptoAlert.SharedKernel.csproj", "src/shared/CryptoAlert.SharedKernel/"]

RUN dotnet restore "src/services/CryptoAlert.PriceCollector/CryptoAlert.PriceCollector.csproj"

COPY . .
RUN dotnet publish "src/services/CryptoAlert.PriceCollector/CryptoAlert.PriceCollector.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CryptoAlert.PriceCollector.dll"]