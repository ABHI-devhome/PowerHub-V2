@echo off
setlocal
cd /d "%~dp0"

set "DOTNET=%ProgramFiles%\dotnet\dotnet.exe"
if not exist "%DOTNET%" set "DOTNET=dotnet"

echo Building PowerHub (Standard and Lite)...
"%DOTNET%" build "%~dp0PowerHub.sln" -c Release

if errorlevel 1 (
  echo.
  echo [Error] Build failed.
  exit /b 1
)

echo.
echo Build succeeded.
echo Standard Executable: src\PowerHub.Wpf\bin\Release\net8.0-windows\PowerHub.exe
echo Lite Executable: src\PowerHub.Lite\bin\Release\net8.0-windows\PowerHubLite.exe
exit /b 0
