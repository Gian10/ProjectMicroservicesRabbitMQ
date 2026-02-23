FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base  
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["OrderConsumer/OrderConsumer.csproj", "OrderConsumer/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "OrderConsumer/OrderConsumer.csproj"

COPY . .
WORKDIR "/src/OrderConsumer"
RUN dotnet build "OrderConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderConsumer.dll"]