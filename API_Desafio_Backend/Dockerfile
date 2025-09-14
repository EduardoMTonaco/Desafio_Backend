FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Desafio_Backend.sln ./
COPY API_Desafio_Backend/ API_Desafio_Backend/

COPY Library_Desafio_Backend/ Library_Desafio_Backend/
COPY MessageBroker_Desafio_Backend/ MessageBroker_Desafio_Backend/
RUN dotnet restore

RUN dotnet publish API_Desafio_Backend/API_Desafio_Backend.csproj -c Release -o /app/publish/api
RUN dotnet publish Library_Desafio_Backend/Library_Desafio_Backend.csproj -c Release -o /app/publish/library
RUN dotnet publish MessageBroker_Desafio_Backend/MessageBroker_Desafio_Backend.csproj -c Release -o /app/publish/MessageBroker_Desafio_Backend

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish/api ./

ENTRYPOINT ["dotnet", "API_Desafio_Backend.dll"]