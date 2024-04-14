### build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /sln

COPY ./*.sln ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore ./Spammer.sln

COPY . .

RUN dotnet build ./Service/Service.csproj -c Release -o /Service/build/
RUN dotnet publish ./Service/Service.csproj -c Release -o /Service/publish/

### sql server build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS service

ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
COPY --from=build /Service/publish .

ENTRYPOINT ["dotnet", "Service.dll"]