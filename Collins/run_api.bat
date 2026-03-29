@echo off
echo Starting Collins-BackEnd API...
dotnet build
dotnet run --project .\Collins-BackEnd.API\Collins-BackEnd.API.csproj
pause
