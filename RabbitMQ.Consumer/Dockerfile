#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RabbitMQ.Consumer/RabbitMQ.Consumer.csproj", "RabbitMQ.Consumer/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "RabbitMQ.Consumer/RabbitMQ.Consumer.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.Consumer"
RUN dotnet build "RabbitMQ.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQ.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQ.Consumer.dll"]
