# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["JtlTask.slnx", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY [".editorconfig", "./"]

# Copy all project files
COPY ["src/JtlTask.WebApi/JtlTask.WebApi.csproj", "src/JtlTask.WebApi/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/Users.Application/Users.Application.csproj", "src/Users.Application/"]
COPY ["src/Users.Domain/Users.Domain.csproj", "src/Users.Domain/"]
COPY ["src/Users.Infrastructure/Users.Infrastructure.csproj", "src/Users.Infrastructure/"]
COPY ["src/WorkItems.Application/WorkItems.Application.csproj", "src/WorkItems.Application/"]
COPY ["src/WorkItems.Domain/WorkItems.Domain.csproj", "src/WorkItems.Domain/"]
COPY ["src/WorkItems.Infrastructure/WorkItems.Infrastructure.csproj", "src/WorkItems.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/JtlTask.WebApi/JtlTask.WebApi.csproj"

# Copy all source code
COPY ["src/", "src/"]

# Build and publish
WORKDIR "/src/src/JtlTask.WebApi"
RUN dotnet build "JtlTask.WebApi.csproj" -c Release -o /app/build
RUN dotnet publish "JtlTask.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create directory for SQLite databases
RUN mkdir -p /app/data

EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JtlTask.WebApi.dll"]