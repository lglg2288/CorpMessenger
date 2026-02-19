FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY ["Server/Server.csproj", "Server/"]
COPY ["GrpcShared/GrpcShared.csproj", "GrpcShared/"]

RUN dotnet restore "Server/Server.csproj"
COPY . .

WORKDIR "/src/Server"
RUN dotnet publish "Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5203
ENV ASPNETCORE_URLS=http://+:5203

ENTRYPOINT ["dotnet", "Server.dll"]