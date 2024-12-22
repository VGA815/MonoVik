FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /src
WORKDIR /src
RUN ls
RUN dotnet build "MonoVik.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonoVik.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonoVik.WebApi.dll"]