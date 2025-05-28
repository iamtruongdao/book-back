# Sử dụng .NET 8.0 runtime Linux (Railway yêu cầu)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src

# Copy csproj và restore dependencies
COPY ["BackEnd.csproj", "./"]
RUN dotnet restore "BackEnd.csproj"

# Copy source code và build
COPY . .
WORKDIR "/src/."
RUN dotnet build "BackEnd.csproj" -c $configuration -o /app/build

# Publish stage
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BackEnd.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackEnd.dll"]