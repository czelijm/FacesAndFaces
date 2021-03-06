#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Ordering/OrdersApi/OrdersApi.csproj", "Ordering/OrdersApi/"]
COPY ["Faces.SharedLib/Messaging.InterfacesConstants/Messaging.InterfacesConstants.csproj", "Faces.SharedLib/Messaging.InterfacesConstants/"]
RUN dotnet restore "Ordering/OrdersApi/OrdersApi.csproj"
COPY . .
WORKDIR "/src/Ordering/OrdersApi"
RUN dotnet build "OrdersApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrdersApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersApi.dll"]
