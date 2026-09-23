@echo off
chcp 65001 >nul
setlocal
set "SERVICE_NAME=DMDService"
set "EXE=%~dp0ASI.Wanda.DMD.DMDService.exe"
set "INSTALLUTIL=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"
if not exist "%INSTALLUTIL%" set "INSTALLUTIL=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe"

net session >nul 2>&1
if errorlevel 1 (
    echo [錯誤] 請在此檔案按右鍵，選擇「以系統管理員身分執行」。
    pause
    exit /b 1
)

sc query %SERVICE_NAME% >nul 2>&1
if errorlevel 1 (
    echo 此電腦沒有安裝 %SERVICE_NAME%。
    pause
    exit /b 0
)

echo 停止服務中 (約 15 秒，服務會先通知各 Task 結束)...
sc stop %SERVICE_NAME% >nul 2>&1
timeout /t 15 /nobreak >nul

echo 移除服務中...
"%INSTALLUTIL%" /u /LogFile= /LogToConsole=false "%EXE%"
if errorlevel 1 (
    echo InstallUtil 移除失敗，改用 sc delete...
    sc delete %SERVICE_NAME%
)

rem 確認沒有殘留的 Task 行程
for %%p in (TaskKernel.exe TaskMain.exe TaskCMFT.exe TaskDCU.exe TaskOCS.exe) do (
    tasklist /fi "imagename eq %%p" | find /i "%%p" >nul && (
        echo 結束殘留行程 %%p
        taskkill /f /im %%p >nul
    )
)

echo [完成] 已移除 %SERVICE_NAME%
pause
