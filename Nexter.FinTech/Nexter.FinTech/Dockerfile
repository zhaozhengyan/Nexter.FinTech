FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Nexter.FinTech/Nexter.FinTech.csproj", "Nexter.FinTech/"]
RUN dotnet restore "Nexter.FinTech/Nexter.FinTech.csproj"
COPY . .
WORKDIR "/src/Nexter.FinTech"
RUN dotnet build "Nexter.FinTech.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Nexter.FinTech.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Nexter.FinTech.dll"]