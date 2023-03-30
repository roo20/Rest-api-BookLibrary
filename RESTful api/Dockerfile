#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RESTful api/RESTful api.csproj", "RESTful api/"]
COPY ["LoggerService/RESTful api.LoggerService.csproj", "LoggerService/"]
COPY ["RESTful api.Contracts/RESTful api.Contracts.csproj", "RESTful api.Contracts/"]
RUN dotnet restore "RESTful api/RESTful api.csproj"
COPY . .
WORKDIR "/src/RESTful api"
RUN dotnet build "RESTful api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RESTful api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RESTful api.dll"]