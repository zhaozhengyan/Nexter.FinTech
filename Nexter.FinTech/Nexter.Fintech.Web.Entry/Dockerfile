#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# .NET 8+ 的 aspnet 基础镜像默认监听 8080（非 root 用户），不再是 80
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY ./ /src
WORKDIR "/src/Nexter.FinTech/Nexter.Fintech.Web.Entry"
RUN dotnet build "Nexter.Fintech.Web.Entry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexter.Fintech.Web.Entry.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexter.Fintech.Web.Entry.dll"]