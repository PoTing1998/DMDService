@echo off
chcp 65001 >nul
setlocal
set "SERVICE_NAME=DMDService"
set "EXE=%~dp0ASI.Wanda.DMD.DMDService.exe"
set "INSTALLUTIL=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"
if not exist "%INSTALLUTIL%" set "INSTALLUTIL=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe"

echo ============================================
echo  安裝 DMDService Windows 服務
echo  位置: %~dp0
echo ============================================
echo.

rem --- 系統管理員權限 ---
net session >nul 2>&1
if errorlevel 1 (
    echo [錯誤] 請在此檔案按右鍵，選擇「以系統管理員身分執行」。
    goto :fail
)

rem --- .NET Framework 4.7.2 以上 (Release >= 461808) ---
set "NETREL="
for /f "tokens=3" %%a in ('reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release 2^>nul ^| find "Release"') do set /a NETREL=%%a
if not defined NETREL (
    echo [錯誤] 找不到 .NET Framework 4.x，請先安裝 .NET Framework 4.7.2 以上版本。
    goto :fail
)
if %NETREL% LSS 461808 (
    echo [錯誤] .NET Framework 版本太舊 ^(Release=%NETREL%^)，需要 4.7.2 以上。
    goto :fail
)
echo [OK] .NET Framework Release=%NETREL%

rem --- MSMQ ---
sc query MSMQ >nul 2>&1
if errorlevel 1 (
    echo [錯誤] 此電腦未安裝 MSMQ ^(Message Queuing^)。
    echo        控制台 ^> 程式和功能 ^> 開啟或關閉 Windows 功能 ^> Microsoft Message Queue ^(MSMQ^) 伺服器
    goto :fail
)
echo [OK] MSMQ 已安裝

if not exist "%EXE%" (
    echo [錯誤] 找不到 %EXE%
    goto :fail
)
if not exist "%~dp0Config\Config.xml" (
    echo [錯誤] 找不到 Config\Config.xml
    goto :fail
)

rem --- 已安裝就先移除 ---
sc query %SERVICE_NAME% >nul 2>&1
if not errorlevel 1 (
    echo 偵測到已安裝的 %SERVICE_NAME%，先停止並移除舊的服務...
    sc stop %SERVICE_NAME% >nul 2>&1
    timeout /t 15 /nobreak >nul
    "%INSTALLUTIL%" /u /LogFile= /LogToConsole=false "%EXE%" >nul
)

echo.
echo 安裝服務中...
"%INSTALLUTIL%" /LogFile= /LogToConsole=false "%EXE%"
if errorlevel 1 (
    echo [錯誤] InstallUtil 安裝失敗。
    goto :fail
)

rem 服務異常結束時 60 秒後自動重啟
sc failure %SERVICE_NAME% reset= 86400 actions= restart/60000/restart/60000/restart/60000 >nul

echo.
set /p START_NOW=要現在啟動服務嗎？(Y/N) 
if /i "%START_NOW%"=="Y" (
    sc start %SERVICE_NAME%
)

echo.
echo [完成] 服務名稱: %SERVICE_NAME%，執行紀錄在 %~dp0Log
echo 注意：服務會從目前這個資料夾執行，安裝後請勿搬移或刪除此資料夾。
pause
exit /b 0

:fail
echo.
pause
exit /b 1
