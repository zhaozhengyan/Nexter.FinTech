
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app

EXPOSE 80

VOLUME ["/log"]

COPY ./ ./

ENV TZ=Asia/Shanghai

ENTRYPOINT ["dotnet", "Nexter.Fintech.Web.Entry.dll"]