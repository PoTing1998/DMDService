@echo off
chcp 65001 >nul
net session >nul 2>&1
if errorlevel 1 (
    echo [錯誤] 請在此檔案按右鍵，選擇「以系統管理員身分執行」。
    pause
    exit /b 1
)
sc start DMDService
timeout /t 3 /nobreak >nul
sc query DMDService | find "STATE"
pause
