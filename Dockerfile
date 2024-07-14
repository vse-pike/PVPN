FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENV ASPNETCORE_URLS=http://+:8080;
EXPOSE 8080
ENTRYPOINT ["dotnet", "PVPN.dll"]