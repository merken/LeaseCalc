FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

COPY LeaseCalc.Insurance.Api ./LeaseCalc.Insurance.Api
COPY LeaseCalc.Contract ./LeaseCalc.Contract

RUN dotnet restore LeaseCalc.Insurance.Api/LeaseCalc.Insurance.Api.csproj
RUN dotnet publish LeaseCalc.Insurance.Api/LeaseCalc.Insurance.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /out ./
EXPOSE 5000
ENTRYPOINT ["dotnet", "LeaseCalc.Insurance.Api.dll"]