@echo off
echo Building Kill The Frog game...
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: .NET is not installed or not in PATH
    echo Please install .NET 6.0 or later from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo .NET version:
dotnet --version
echo.

echo Building project...
dotnet build -c Release

if %errorlevel% equ 0 (
    echo.
    echo ✓ Build successful!
    echo.
    echo You can now run the game with:
    echo   dotnet run
    echo.
    echo Or create a standalone executable with:
    echo   dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
    echo.
) else (
    echo.
    echo ✗ Build failed!
    echo Check the error messages above.
)

pause