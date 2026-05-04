@echo off
setlocal EnableExtensions
cd /d "%~dp0"

set "CONFIG=Release"
if not "%~1"=="" set "CONFIG=%~1"

where cmake >nul 2>&1
if errorlevel 1 (
  echo [ERROR] CMake not found in PATH.
  exit /b 1
)

set "SRC_DIR=%cd%\native_scanner"
set "BUILD_DIR=%cd%\build_native_scanner"
set "APP_OUT=%cd%\bin\%CONFIG%\net8.0-windows"
set "RULES_FILE=%SRC_DIR%\rules.json"

if not exist "%SRC_DIR%\CMakeLists.txt" (
  echo [ERROR] Native scanner sources not found: %SRC_DIR%
  exit /b 1
)

echo [INFO] Configure native scanner...
cmake -S "%SRC_DIR%" -B "%BUILD_DIR%"
if errorlevel 1 (
  echo [ERROR] CMake configure failed.
  exit /b 1
)

echo [INFO] Build native scanner DLL (%CONFIG%)...
cmake --build "%BUILD_DIR%" --config "%CONFIG%" --target scum_native_scanner
if errorlevel 1 (
  echo [ERROR] Native scanner build failed.
  exit /b 1
)

set "DLL_PATH=%BUILD_DIR%\%CONFIG%\scum_native_scanner.dll"
if not exist "%DLL_PATH%" (
  set "DLL_PATH=%BUILD_DIR%\scum_native_scanner\%CONFIG%\scum_native_scanner.dll"
)

if exist "%DLL_PATH%" (
  echo [OK] Native DLL built:
  echo [OUT] %DLL_PATH%

  if exist "%APP_OUT%" (
    copy /Y "%DLL_PATH%" "%APP_OUT%\scum_native_scanner.dll" >nul
    if exist "%RULES_FILE%" copy /Y "%RULES_FILE%" "%APP_OUT%\rules.json" >nul
    echo [OK] Copied to app output:
    echo [OUT] %APP_OUT%\scum_native_scanner.dll
    if exist "%RULES_FILE%" echo [OUT] %APP_OUT%\rules.json
  ) else (
    echo [WARN] App output folder not found yet:
    echo [WARN] %APP_OUT%
    echo [INFO] Build ScumChecker first: build_scumchecker_dll.bat %CONFIG%
  )
) else (
  echo [ERROR] Built target but DLL file not found.
  exit /b 1
)

exit /b 0
