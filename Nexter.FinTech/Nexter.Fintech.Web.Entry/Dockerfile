#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ./ /src
RUN ls -la
RUN cd src/
RUN ls -la
COPY ["Nexter.Fintech.Web.Entry/Nexter.Fintech.Web.Entry.csproj", "Nexter.Fintech.Web.Entry/"]
COPY ["Nexter.Fintech.Web.Core/Nexter.Fintech.Web.Core.csproj", "Nexter.Fintech.Web.Core/"]
COPY ["Nexter.Fintech.Database.Migrations/Nexter.Fintech.Database.Migrations.csproj", "Nexter.Fintech.Database.Migrations/"]
COPY ["Nexter.Fintech.EntityFramework.Core/Nexter.Fintech.EntityFramework.Core.csproj", "Nexter.Fintech.EntityFramework.Core/"]
COPY ["Nexter.Fintech.Core/Nexter.Fintech.Core.csproj", "Nexter.Fintech.Core/"]
COPY ["Nexter.Fintech.Application/Nexter.Fintech.Application.csproj", "Nexter.Fintech.Application/"]
RUN dotnet restore "Nexter.Fintech.Web.Entry/Nexter.Fintech.Web.Entry.csproj"
COPY . .
WORKDIR "/src/Nexter.Fintech.Web.Entry"
RUN ls -la
RUN dotnet build "Nexter.Fintech.Web.Entry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexter.Fintech.Web.Entry.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN ls -la
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexter.Fintech.Web.Entry.dll"]