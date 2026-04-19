FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CryptoAlertSystem.sln", "./"]
COPY ["src/services/CryptoAlert.Notifications/CryptoAlert.Notifications.csproj", "src/services/CryptoAlert.Notifications/"]
COPY ["src/shared/CryptoAlert.Contracts/CryptoAlert.Contracts.csproj", "src/shared/CryptoAlert.Contracts/"]
COPY ["src/shared/CryptoAlert.SharedKernel/CryptoAlert.SharedKernel.csproj", "src/shared/CryptoAlert.SharedKernel/"]
COPY ["src/persistence/CryptoAlert.Database/CryptoAlert.Database.csproj", "src/persistence/CryptoAlert.Database/"]

RUN dotnet restore "src/services/CryptoAlert.Notifications/CryptoAlert.Notifications.csproj"

COPY . .
RUN dotnet publish "src/services/CryptoAlert.Notifications/CryptoAlert.Notifications.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CryptoAlert.Notifications.dll"]