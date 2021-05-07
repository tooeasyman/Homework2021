FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster as dev
WORKDIR /src
COPY ./wia.csproj .
RUN dotnet restore wia.csproj
COPY . .
RUN dotnet build wia.csproj -c Debug -o /src/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=dev /src/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "wia.dll"]
