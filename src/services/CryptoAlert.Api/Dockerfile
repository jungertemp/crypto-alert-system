FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CryptoAlertSystem.sln", "./"]
COPY ["src/services/CryptoAlert.Api/CryptoAlert.Api.csproj", "src/services/CryptoAlert.Api/"]
COPY ["src/shared/CryptoAlert.Contracts/CryptoAlert.Contracts.csproj", "src/shared/CryptoAlert.Contracts/"]
COPY ["src/shared/CryptoAlert.SharedKernel/CryptoAlert.SharedKernel.csproj", "src/shared/CryptoAlert.SharedKernel/"]
COPY ["src/persistence/CryptoAlert.Database/CryptoAlert.Database.csproj", "src/persistence/CryptoAlert.Database/"]

RUN dotnet restore "src/services/CryptoAlert.Api/CryptoAlert.Api.csproj"

COPY . .
RUN dotnet publish "src/services/CryptoAlert.Api/CryptoAlert.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CryptoAlert.Api.dll"]