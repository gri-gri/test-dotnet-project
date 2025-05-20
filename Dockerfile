FROM mcr.microsoft.com/dotnet/sdk:9.0.300-alpine3.21@sha256:2244f80ac7179b0feaf83ffca8fe82d31fbced5b7e353755bf9515a420eba711 AS build
WORKDIR /src

COPY src/TestDotnetProject/*.csproj ./TestDotnetProject/
RUN dotnet restore ./TestDotnetProject/

COPY src/ ./
RUN dotnet publish -c Release -o /app ./TestDotnetProject/


FROM mcr.microsoft.com/dotnet/aspnet:9.0.5-alpine3.21@sha256:30fdbd1b5963bba6ed66190d72d877b750d4203a671c9b54592f4551b8c5a087 AS final
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "TestDotnetProject.dll" ]
