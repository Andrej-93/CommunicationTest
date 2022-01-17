#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RabbitMQ.ConsumerTest/RabbitMQ.ConsumerTest.csproj", "RabbitMQ.ConsumerTest/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "RabbitMQ.ConsumerTest/RabbitMQ.ConsumerTest.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.ConsumerTest"
RUN dotnet build "RabbitMQ.ConsumerTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQ.ConsumerTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQ.ConsumerTest.dll"]
