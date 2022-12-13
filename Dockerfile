FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /Quotation.Management.API

COPY *.sln .
COPY Quotation.Management.API/*.CSPROJ ./Quotation.Management.API/
RUN dotnet restore

COPY Quotation.Management.API/. ./Quotation.Management.API/
WORKDIR /repos/QMT-API
RUN dotnet publish -c release -o/app --no--restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS Runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Quotation.Management.API.dll"]