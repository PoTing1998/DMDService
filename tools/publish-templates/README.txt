DMDService 發佈包
==================================================

本資料夾是建置後的執行檔，不含程式碼。建置資訊（時間、版本）請看 BuildInfo.txt。

資料夾內容
--------------------------------------------------
DMDService\          正式服務 (Windows 服務 + 各 Task)
  ASI.Wanda.DMD.DMDService.exe   Windows 服務本體 (服務名稱：DMDService)
  TaskKernel.exe / TaskMain.exe  由服務自動啟動，不要手動執行
  TaskCMFT.exe / TaskDCU.exe / TaskOCS.exe
  Config\Config.xml              ★ 執行環境設定，交付前請確認
  Install-Service.bat            安裝並啟動服務 (右鍵 → 以系統管理員身分執行)
  Uninstall-Service.bat          停止並移除服務
  Start-Service.bat / Stop-Service.bat
  Service-Status.bat             查看服務、Task 行程、監聽中的通訊埠
  Log\                           執行後自動產生的紀錄檔

UITest\              測試工具，直接執行 UITest.exe 即可 (不需安裝)


執行環境需求
--------------------------------------------------
1. Windows 10 / Windows Server 2016 以上
2. .NET Framework 4.7.2 以上
3. MSMQ (Message Queuing)：各 Task 之間用它溝通，服務必須要有
   控制台 > 程式和功能 > 開啟或關閉 Windows 功能 > Microsoft Message Queue (MSMQ) 伺服器
   (Windows Server：伺服器管理員 > 新增角色及功能 > 功能 > Message Queuing)
4. 能連到 PostgreSQL：DMDDB、CMFTDB (IP 在 Config.xml 設定)


安裝服務
--------------------------------------------------
1. 把整個 DMDService 資料夾複製到目標電腦的固定位置，例如 D:\ASI.Wanda.DMD.DMDService
   ※ 服務會從這個資料夾執行，安裝後不要搬移、刪除或覆蓋 (更新前請先停止服務)
2. 編輯 Config\Config.xml (見下方)
3. 在 Install-Service.bat 按右鍵 → 以系統管理員身分執行
   會檢查 .NET、MSMQ、系統管理員權限後安裝，並設定異常結束 60 秒後自動重啟
4. 用 Service-Status.bat 確認 TaskCMFT / TaskDCU / TaskOCS 都有在執行，
   而且 2000、8000 port 在 LISTENING

更新版本：Stop-Service.bat → 覆蓋 exe/dll (保留自己的 Config\Config.xml) → Start-Service.bat
移除服務：Uninstall-Service.bat


Config\Config.xml 主要設定
--------------------------------------------------
Host_Name                 主機名稱
STATION_ID                本機站所代碼 (OCC / BOCC / LG01~LG08A)
DMD_DB_IP / Port / Name   DMD 資料庫
DMD_DB_userID / Passward  DMD 資料庫帳密 (未設定時使用 postgres)
CMFT_DB_IP / Port / Name  CMFT 資料庫
CMFT_DB_userID / Passward CMFT 資料庫帳密
DCU_Server                各車站 DCU 連入的 Socket，例：IP=;Port=2000;Type=Server
CMFT_Server               CMFT 連入的 Socket，例：IP=;Port=8000;Type=Server
                          (Type=Server 時 IP 是本機監聽的網卡，留空 = 所有網卡)
CMFT_Client_IP            預期連入的 CMFT IP (不符只記警告)
CMFT_Heartbeat_Interval   送給 CMFT 的 Heartbeat 間隔秒數 (0 = 不送)
CMFT_Heartbeat_Timeout    多久沒收到 CMFT 資料就斷開該連線 (0 = 不檢查)
TcpClientIP / TcpClientPort  OCS Modbus 連線
Socket_Admin_IP           連線監控管理通道綁定的 IP，預設 127.0.0.1 (只允許本機)
Socket_Admin_CMFT_Port    TaskCMFT 管理通道 port，預設 18000
Socket_Admin_DCU_Port     TaskDCU  管理通道 port，預設 18001

各車站 DCU 的 IP 以資料庫 dbo.station_conf.dcu_ip 為準 (Config 內的 DCU_LGxx 僅供對照)。


防火牆 / 通訊埠
--------------------------------------------------
輸入  TCP 2000   各車站 DCU 連入
輸入  TCP 8000   CMFT 連入
輸出  TCP 5432   PostgreSQL
輸出  TCP 502    OCS Modbus
本機  TCP 18000 / 18001  UITest 連線監控用 (預設只綁 127.0.0.1，不需開防火牆)


UITest
--------------------------------------------------
直接執行 UITest\UITest.exe。
「Socket 連線監控」分頁：
  - 在服務所在的電腦執行，服務主機填 127.0.0.1，管理埠 18000 (CMFT) / 18001 (DCU)
  - 可看 9 個 DCU 車站與 CMFT 的連線狀態、連線/斷線紀錄，強制斷線與發送測試訊息
  - 要從別台電腦監控，需把 Config.xml 的 Socket_Admin_IP 改成 0.0.0.0 並開放防火牆；
    管理通道沒有密碼，任何連得到的人都能斷線或送訊息，請謹慎使用
其他分頁 (Task CMFT / Task DCU / 降級營運模式...) 是開發測試用，會自行開 Socket 或連資料庫，
請不要在正式服務執行中的電腦上用相同 port 開啟，以免衝突。


問題排查
--------------------------------------------------
- 服務啟動後馬上停止：看 DMDService\Log\<日期>\ 內的 Error 紀錄
- 資料庫連線失敗：確認 Config.xml 的 DB IP / 帳密，以及 5432 port 可連
- 某站 DCU 一直離線：UITest「DCU 車站總覽」確認來源 IP 與 station_conf.dcu_ip 是否一致
