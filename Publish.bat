@echo off
rem Build the solution and collect runnable files into Publish\ (DMDService + UITest, no source code).
rem Arguments are passed to tools\Publish.ps1, e.g.:
rem   Publish.bat -Configuration Debug -IncludePdb -Zip
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Publish.ps1" %*
if errorlevel 1 (
    echo.
    echo Publish failed.
)
pause
