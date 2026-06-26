@echo off
chcp 65001 >nul
title ScumChecker • Git Push

echo ===============================
echo  ScumChecker - Git Push
echo ===============================
echo.

REM Перейти в папку репозитория
cd /d "%~dp0"

REM Проверка, что это git-репо
if not exist ".git" (
    echo [ERROR] This is not a git repository.
    pause
    exit /b
)

REM Добавить все изменения
echo [INFO] Adding files...
git add .

REM Коммит
set msg=Update
if not "%~1"=="" set msg=%~1

echo [INFO] Commit message: "%msg%"
git commit -m "%msg%"

REM Пуш
echo [INFO] Pushing to GitHub...
git push

echo.
echo [DONE] Push completed.
pause
