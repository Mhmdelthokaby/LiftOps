@echo off
echo Starting LiftOps-BackEnd API...
dotnet build
dotnet run --project .\LiftOps-BackEnd.API\LiftOps-BackEnd.API.csproj
pause
