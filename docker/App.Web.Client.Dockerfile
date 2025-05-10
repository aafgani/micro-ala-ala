# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 

WORKDIR /app

COPY ./apps/App.Web.Client/published/ .

ENTRYPOINT [ "dotnet", "App.Web.Client.dll" ]