FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Nexter.FinTech/FinTech.API.Wechat.csproj", "Nexter.FinTech/"]
RUN dotnet restore "Nexter.FinTech/FinTech.API.Wechat.csproj"
COPY . .
WORKDIR "/src/Nexter.FinTech"
RUN dotnet build "FinTech.API.Wechat.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FinTech.API.Wechat.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Nexter.FinTech.dll"]