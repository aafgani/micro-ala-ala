# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
WORKDIR /src

# Copy project files
COPY "../apps/App.Web.Client/App.Web.Client.csproj" "apps/App.Web.Client/"
COPY "../shared/App.Common.Abstractions/App.Common.Abstractions.csproj" "shared/App.Common.Abstractions/"
COPY "../infrastructure/App.Common.Infrastructure/App.Common.Infrastructure.csproj" "infrastructure/App.Common.Infrastructure/"
COPY "../shared/App.Common.Model/App.Common.Domain.csproj" "shared/App.Common.Model/"

# Copy the rest of the source code
COPY "../apps/App.Web.Client/" "apps/App.Web.Client/"
COPY "../shared/App.Common.Abstractions/" "shared/App.Common.Abstractions/"
COPY "../infrastructure/App.Common.Infrastructure/" "infrastructure/App.Common.Infrastructure/"
COPY "../shared/App.Common.Model/" "shared/App.Common.Model/"

# Publish the application
RUN dotnet publish "apps/App.Web.Client/App.Web.Client.csproj" -c Release -o /app/published /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0  
WORKDIR /app
COPY --from=build ./app/published .
ENTRYPOINT [ "dotnet", "App.Web.Client.dll" ]