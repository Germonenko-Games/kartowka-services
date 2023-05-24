FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Kartowka.Api/Kartowka.Api.csproj", "Kartowka.Api/"]
RUN dotnet restore "src/Kartowka.Api/Kartowka.Api.csproj"
COPY . .
WORKDIR "/src/Kartowka.Api"
RUN dotnet build "Kartowka.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kartowka.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kartowka.Api.dll"]