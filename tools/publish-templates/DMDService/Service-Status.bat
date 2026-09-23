@echo off
chcp 65001 >nul
echo ===== 服務狀態 =====
sc query DMDService
echo.
echo ===== 執行中的 Task =====
tasklist | findstr /i "ASI.Wanda.DMD.DMDService TaskKernel TaskMain TaskCMFT TaskDCU TaskOCS"
echo.
echo ===== 監聽中的通訊埠 (2000=DCU, 8000=CMFT, 18000/18001=連線監控) =====
netstat -ano | findstr /r /c:":2000 .*LISTENING" /c:":8000 .*LISTENING" /c:":18000 .*LISTENING" /c:":18001 .*LISTENING"
echo.
pause
