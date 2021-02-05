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

RUN apt-get update && apt-get install -y libcgroup-dev gcc g++ default-jdk nodejs python3 wget unzip

RUN cd /usr/lib && \
    wget -q https://github.com/JetBrains/kotlin/releases/download/v1.4.10/kotlin-compiler-1.4.10.zip && \
    unzip kotlin-compiler-*.zip && \
    rm kotlin-compiler-*.zip && \
    rm -f kotlinc/bin/*.bat

ENV PATH $PATH:/usr/lib/kotlinc/bin

COPY ./java.policy /etc/java-11-openjdk/security/java.policy

WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=run-guard /usr/runguard ./Sandbox/RunGuard/runguard

ENTRYPOINT ["dotnet", "JobeSharp.dll"]