FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CreateOrder/CreateOrder.csproj", "CreateOrder/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "CreateOrder/CreateOrder.csproj"

COPY . .
WORKDIR "/src/CreateOrder"
RUN dotnet build "CreateOrder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreateOrder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreateOrder.dll"]