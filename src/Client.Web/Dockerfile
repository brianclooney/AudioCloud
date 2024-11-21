FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Client.Web/*.csproj ./Client.Web/
COPY Shared/*.csproj ./Shared/
RUN dotnet restore ./Client.Web/Client.Web.csproj

# Copy everything else and build
COPY Client.Web/ ./Client.Web/
COPY Shared/ ./Shared/
RUN dotnet publish ./Client.Web/Client.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /src/out .
RUN rm appsettings.Development.json
ENTRYPOINT ["dotnet", "Client.Web.dll"]