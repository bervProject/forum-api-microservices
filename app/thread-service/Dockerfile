FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o published

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY --from=build /app/published .
ENTRYPOINT [ "dotnet", "ThreadService.dll" ]
