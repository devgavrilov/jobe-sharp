#sudo g++ ./Sandbox/RunGuard/runguard.c -o ./Sandbox/RunGuard/runguard -lcgroup

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal

RUN apt-get update && apt-get install -y gcc default-jdk

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "JobeSharp.dll"]