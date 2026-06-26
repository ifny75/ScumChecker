@echo off
setlocal EnableExtensions
cd /d "%~dp0"

set "CONFIG=Release"
if not "%~1"=="" set "CONFIG=%~1"

where dotnet >nul 2>&1
if errorlevel 1 (
  echo [ERROR] dotnet SDK not found in PATH.
  exit /b 1
)

echo [INFO] Building ScumChecker (%CONFIG%)...
dotnet build ScumChecker.csproj -c %CONFIG% /nologo
if errorlevel 1 (
  echo [ERROR] Build failed.
  exit /b 1
)

set "OUT_DIR=%cd%\bin\%CONFIG%\net8.0-windows"
set "DLL_PATH=%OUT_DIR%\ScumChecker.dll"

if exist "%DLL_PATH%" (
  echo [OK] DLL built:
  echo [OUT] %DLL_PATH%
) else (
  echo [WARN] Build finished, but DLL not found:
  echo [WARN] %DLL_PATH%
)

if exist "%cd%\build_native_scanner_dll.bat" (
  echo [INFO] Building native scanner DLL...
  call "%cd%\build_native_scanner_dll.bat" %CONFIG%
  if errorlevel 1 (
    echo [WARN] Native scanner build failed. Managed DLL is still built.
  )
)

exit /b 0
