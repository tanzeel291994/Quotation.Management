FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./Quotation.Management.API/Quotation.Management.API.csproj" --disable-parallel
RUN dotnet publish "./Quotation.Management.API/Quotation.Management.API.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS Runtime
WORKDIR /app
COPY --from=build /app ./
RUN ls -al
ENTRYPOINT ["dotnet", "Quotation.Management.API.dll", "--urls", "http://*:5005"]
