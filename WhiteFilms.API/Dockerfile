#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WhiteFilms.API/WhiteFilms.API.csproj", "WhiteFilms.API/"]
RUN dotnet restore "WhiteFilms.API/WhiteFilms.API.csproj"
COPY . .
WORKDIR "/src/WhiteFilms.API"
RUN dotnet build "WhiteFilms.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhiteFilms.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhiteFilms.API.dll"]