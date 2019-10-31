FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
COPY ./ /src
RUN ls -la 
WORKDIR "/src/Nexter.FinTech/FinTech.API.Wechat"
FROM build AS publish
RUN ls -la 
RUN dotnet publish "FinTech.API.Wechat.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FinTech.API.Wechat.dll"]



