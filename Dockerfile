FROM gcc:latest AS run-guard
COPY ./Sandbox/RunGuard/* /usr/src/
RUN apt-get update && apt-get install -y libcgroup-dev
RUN g++ -o /usr/runguard /usr/src/runguard.c -lcgroup

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal

RUN apt-get update && apt-get install -y libcgroup-dev gcc default-jdk nodejs

WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=run-guard /usr/runguard ./Sandbox/RunGuard/runguard

ENTRYPOINT ["dotnet", "JobeSharp.dll"]