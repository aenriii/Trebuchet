FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Trebuchet.Server/Trebuchet.Server.csproj", "Trebuchet.Server/"]
RUN dotnet restore "Trebuchet.Server/Trebuchet.Server.csproj"
COPY . .
WORKDIR "/src/Trebuchet.Server"
RUN dotnet build "Trebuchet.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trebuchet.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trebuchet.Server.dll"]
