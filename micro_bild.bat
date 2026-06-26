@echo off
setlocal
cd /d "%~dp0"

echo ==============================
echo Building ScumChecker LIGHT
echo ==============================

dotnet publish ScumChecker.csproj -c Release -r win-x64 ^
  /p:SelfContained=false ^
  /p:PublishSingleFile=true ^
  /p:DebugType=None ^
  /p:DebugSymbols=false ^
  -o release_light



echo.
echo DONE!
echo Output:
echo %cd%\release_light
pause
