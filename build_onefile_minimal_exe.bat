@echo off
setlocal EnableExtensions
cd /d "%~dp0"

set "CONFIG=Release"
set "RID=win-x64"
set "OUT_DIR=%cd%\publish_onefile_min"
set "MODE=safe"
if /I "%~1"=="ultra" set "MODE=ultra"

where dotnet >nul 2>&1
if errorlevel 1 (
  echo [ERROR] dotnet SDK not found in PATH.
  exit /b 1
)

if exist "%OUT_DIR%" rmdir /s /q "%OUT_DIR%"

set "PUBLISH_ARGS=-c %CONFIG% -r %RID% --self-contained false -o "%OUT_DIR%""
set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:PublishSingleFile=true"
set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:PublishReadyToRun=false"
set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:DebugType=None -p:DebugSymbols=false"
set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:EnableCompressionInSingleFile=false"
set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:IncludeAllContentForSelfExtract=false"

if /I "%MODE%"=="ultra" (
  echo [INFO] Mode: ULTRA (smaller size, may break some WinForms features)
  set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:PublishTrimmed=true -p:TrimMode=partial"
) else (
  echo [INFO] Mode: SAFE (single EXE, more stable)
  set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:PublishTrimmed=false"
)

echo [INFO] Publishing single-file EXE...
call dotnet publish ScumChecker.csproj %PUBLISH_ARGS% /nologo
if errorlevel 1 (
  echo [ERROR] Publish failed.
  exit /b 1
)

set "EXE_PATH=%OUT_DIR%\ScumChecker.exe"
if not exist "%EXE_PATH%" (
  echo [ERROR] EXE not found: %EXE_PATH%
  exit /b 1
)

for %%A in ("%EXE_PATH%") do set "EXE_SIZE=%%~zA"

echo [OK] Single EXE ready:
echo [OUT] %EXE_PATH%
echo [SIZE] %EXE_SIZE% bytes

echo.
echo [NOTE] This is framework-dependent. .NET Desktop Runtime 8 is required on target PC.
exit /b 0
