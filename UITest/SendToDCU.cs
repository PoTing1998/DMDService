using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASI.Wanda.DMD;

namespace UITest
{
    public partial class SendToDCU : UserControl
    {
        #region Fields
        private ASI.Wanda.DMD.DMD_API mDMD_API = null;
        private Random random = new Random();
        private ASI.Wanda.DMD.DB.Models.dmd_instant_message currentInstantMessage = null;
        #endregion

        #region Constructor
        public SendToDCU()
        {
            InitializeComponent();
            InitializeUI();
            mDMD_API = new DMD_API();
        }
        #endregion

        #region Initialize
        private void InitializeUI()
        {
            // 設定預設值 - DMD 連線
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8001";
            cboType.Items.AddRange(new object[] { "Server", "Client" });
            cboType.SelectedIndex = 0; // 預設 Server

            // 設定預設值 - 資料庫連線（已在 Designer 中設定）
            // txtDBIP.Text = "127.0.0.1";
            // txtDBPort.Text = "5432";
            // txtDBName.Text = "DMDDB";
            // txtDBUserID.Text = "postgres";
            // txtDBPassword.Text = "postgres";

            // 訊息類型下拉選單
            cboMessageType.Items.AddRange(new object[] {
                "預錄訊息 (SendPreRecordMessage)",
                "即時訊息 (SendInstantMessage)"
            });
            cboMessageType.SelectedIndex = 0;
            cboMessageType.SelectedIndexChanged += cboMessageType_SelectedIndexChanged;

            // 車站下拉選單
            cboStation.Items.AddRange(new object[] {
                "全部", "LG01", "LG02", "LG03", "LG04", "LG05",
                "LG06", "LG07", "LG08", "LG08A"
            });
            cboStation.SelectedIndex = 0;

            // 優先等級下拉選單 (1~5)
            cboPriority.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cboPriority.SelectedIndex = 2; // 預設選擇 3

            // 移動速度下拉選單 (1~5)
            cboMoveSpeed.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cboMoveSpeed.SelectedIndex = 2; // 預設選擇 3

            // 移動方式下拉選單
            cboMoveMode.Items.AddRange(new object[] {
                "0-立即",
                "1-靜態",
                "2-上移",
                "3-下移",
                "4-左移",
                "5-右移",
                "6-閃爍"
            });
            cboMoveMode.SelectedIndex = 0; // 預設選擇 立即

            // 初始化日誌
            txtLog.Text = "=== 發送至 DCU 日誌 ===\r\n";
            AppendLog("提示：請先點擊「連線資料庫」載入訊息");

            // 初始化 ListView 列頭
            InitializeListViewColumns();
        }

        private void InitializeListViewColumns()
        {
            clbTargetDU.Columns.Clear();
            clbTargetDU.Columns.Add("車站", 80);
            clbTargetDU.Columns.Add("區域", 80);
            clbTargetDU.Columns.Add("設備ID", 200);
        }

        private void InitializeDatabase(string dbIP, string dbPort, string dbName, string userID, string password)
        {
            try
            {
                // 初始化 DMD 資料庫連線
                bool result = ASI.Wanda.DMD.DB.Manager.Initializer(
                    dbIP,           // Host
                    dbPort,         // Port
                    dbName,         // Database
                    userID,         // UserID
                    password,       // Password
                    "DMDServer"     // CurrentUserID
                );

                if (result)
                {
                    AppendLog($"✓ 資料庫連線成功: {dbIP}:{dbPort}/{dbName}");
                    LoadMessageIDsFromDatabase();
                    // 根據當前選擇的車站載入目標看板
                    string selectedStation = cboStation.SelectedItem?.ToString();
                    LoadTargetDUFromDatabase(selectedStation);
                }
                else
                {
                    AppendLog("✗ 資料庫連線失敗");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"資料庫初始化錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void LoadMessageIDsFromDatabase()
        {
            try
            {
                // 從資料庫讀取所有預錄訊息
                var messages = ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.SelectAll();

                if (messages != null && messages.Count > 0)
                {
                    // 清空現有項目
                    cboMessageID.Items.Clear();
                    cboMessageIDEn.Items.Clear();

                    // 設定 ComboBox 的顯示成員
                    cboMessageID.DisplayMember = "message_content";
                    cboMessageID.ValueMember = "message_id";
                    cboMessageIDEn.DisplayMember = "message_content_en";
                    cboMessageIDEn.ValueMember = "message_id";

                    // 添加所有 message 物件到兩個下拉選單
                    foreach (var message in messages)
                    {
                        cboMessageID.Items.Add(message);
                        cboMessageIDEn.Items.Add(message);
                    }

                    // 預設選擇第一個
                    if (cboMessageID.Items.Count > 0)
                    {
                        cboMessageID.SelectedIndex = 0;
                        cboMessageIDEn.SelectedIndex = 0;
                    }

                    AppendLog($"✓ 已載入 {cboMessageID.Items.Count} 個訊息");
                }
                else
                {
                    // 沒有訊息時，清空下拉選單
                    cboMessageID.Items.Clear();
                    cboMessageIDEn.Items.Clear();
                    AppendLog("⚠ 資料庫中沒有預錄訊息");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"載入訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void LoadTargetDUFromDatabase(string stationFilter = null)
        {
            try
            {
                // 從資料庫讀取所有目標看板
                var playlists = ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.SelectAll();

                if (playlists != null && playlists.Count > 0)
                {
                    // 清空現有項目
                    clbTargetDU.Items.Clear();

                    // 建立分組結構：車站 → 區域 → 設備列表
                    var groupedData = new Dictionary<string, Dictionary<string, List<string>>>();

                    // 組合 station_id_area_id_device_id 格式並分組
                    foreach (var playlist in playlists)
                    {
                        // 如果有車站篩選條件，則只添加符合條件的看板
                        if (!string.IsNullOrEmpty(stationFilter))
                        {
                            // 特殊處理：全部
                            if (stationFilter == "全部")
                            {
                                // 排除 OCC
                                if (playlist.station_id == "OCC")
                                {
                                    continue; // 跳過 OCC
                                }
                            }
                            else
                            {
                                // 檢查 station_id 是否符合篩選條件
                                if (playlist.station_id != stationFilter)
                                {
                                    continue; // 跳過不符合的看板
                                }
                            }
                        }

                        string stationId = playlist.station_id;
                        string areaId = playlist.area_id;
                        string target = $"{playlist.station_id}_{playlist.area_id}_{playlist.device_id}";

                        // 建立車站分組
                        if (!groupedData.ContainsKey(stationId))
                        {
                            groupedData[stationId] = new Dictionary<string, List<string>>();
                        }

                        // 建立區域分組
                        if (!groupedData[stationId].ContainsKey(areaId))
                        {
                            groupedData[stationId][areaId] = new List<string>();
                        }

                        // 添加設備（避免重複）
                        if (!groupedData[stationId][areaId].Contains(target))
                        {
                            groupedData[stationId][areaId].Add(target);
                        }
                    }

                    // 按照分組添加到 ListView
                    int totalDevices = 0;
                    foreach (var station in groupedData.OrderBy(s => s.Key))
                    {
                        foreach (var area in station.Value.OrderBy(a => a.Key))
                        {
                            // 添加設備項目（默認勾選）
                            foreach (var device in area.Value.OrderBy(d => d))
                            {
                                var deviceItem = new ListViewItem(station.Key);
                                deviceItem.SubItems.Add(area.Key);
                                deviceItem.SubItems.Add(device);
                                deviceItem.Tag = device; // 保存原始設備ID
                                deviceItem.Checked = true; // 默認勾選
                                clbTargetDU.Items.Add(deviceItem);
                                totalDevices++;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(stationFilter))
                    {
                        AppendLog($"✓ 已載入 {totalDevices} 個目標看板 (車站: {stationFilter})");
                    }
                    else
                    {
                        AppendLog($"✓ 已載入 {totalDevices} 個目標看板");
                    }
                }
                else
                {
                    AppendLog("⚠ 資料庫中沒有播放清單資料");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"載入目標看板錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }
        #endregion

        #region DMD API Events
        private void DMD_API_ReceivedEvent(ASI.Wanda.DMD.Message.Message DCUServerMessage)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string jsonData = DCUServerMessage.JsonContent;
                string jsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(jsonData, "JsonObjectName");

                if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Ack)
                {
                    AppendLog($"[{timestamp}] 收到 ACK，訊息ID: {DCUServerMessage.MessageID}");
                }
                else if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Response)
                {
                    AppendLog($"[{timestamp}] 收到回應: {jsonObjectName}");
                    AppendLog($"內容: {jsonData}");
                }
                else if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Command)
                {
                    AppendLog($"[{timestamp}] 收到命令: {jsonObjectName}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"接收訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }
        #endregion

        #region Button Events
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                    mDMD_API.Dispose();
                }

                string connString = $"IP={txtIP.Text};Port={txtPort.Text};Type={cboType.Text}";
                mDMD_API = new DMD_API();
                mDMD_API.ReceivedEvent += DMD_API_ReceivedEvent;

                int result = mDMD_API.Initial(connString);

                if (result == 0)
                {
                    AppendLog($"✓ 連線成功: {connString}");
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    btnSend.Enabled = true;
                }
                else
                {
                    AppendLog($"✗ 連線失敗，錯誤碼: {result}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"連線錯誤: {ex.Message}");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                    mDMD_API.Dispose();
                    mDMD_API = null;
                }

                AppendLog("✓ 已斷開連線");
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
            }
            catch (Exception ex)
            {
                AppendLog($"斷線錯誤: {ex.Message}");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                int messageType = cboMessageType.SelectedIndex;

                switch (messageType)
                {
                    case 0: SendPreRecordMessage(); break;
                    case 1: SendInstantMessage(); break;
                    default:
                        AppendLog("請選擇訊息類型");
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"發送錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Text = "=== 發送至 DCU 日誌 ===\r\n";
        }

        private void btnExportLog_Click(object sender, EventArgs e)
        {
            try
            {
                // 創建 SaveFileDialog
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                    saveFileDialog.Title = "匯出 DCU 日誌";
                    saveFileDialog.FileName = $"DCU_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 寫入日誌內容到檔案
                        System.IO.File.WriteAllText(saveFileDialog.FileName, txtLog.Text, Encoding.UTF8);
                        AppendLog($"✓ 日誌已匯出至: {saveFileDialog.FileName}");
                        MessageBox.Show($"日誌已成功匯出至:\n{saveFileDialog.FileName}", "匯出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"匯出日誌錯誤: {ex.Message}");
                MessageBox.Show($"匯出日誌時發生錯誤:\n{ex.Message}", "匯出失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void btnRefreshMessageID_Click(object sender, EventArgs e)
        {
            AppendLog("正在重新載入訊息...");

            int messageType = cboMessageType.SelectedIndex;

            if (messageType == 0) // 預錄訊息
            {
                LoadMessageIDsFromDatabase();
            }
            else if (messageType == 1) // 即時訊息
            {
                LoadInstantMessageIDsFromDatabase();
            }
        }

        private void btnConnectDB_Click(object sender, EventArgs e)
        {
            try
            {
                // 驗證輸入
                if (string.IsNullOrWhiteSpace(txtDBIP.Text))
                {
                    AppendLog("錯誤：請輸入資料庫 IP");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDBPort.Text))
                {
                    AppendLog("錯誤：請輸入資料庫 Port");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDBName.Text))
                {
                    AppendLog("錯誤：請輸入資料庫名稱");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDBUserID.Text))
                {
                    AppendLog("錯誤：請輸入使用者帳號");
                    return;
                }

                // 連線資料庫
                AppendLog($"正在連線至資料庫 {txtDBIP.Text}:{txtDBPort.Text}/{txtDBName.Text}...");

                InitializeDatabase(
                    txtDBIP.Text,
                    txtDBPort.Text,
                    txtDBName.Text,
                    txtDBUserID.Text,
                    txtDBPassword.Text
                );
            }
            catch (Exception ex)
            {
                AppendLog($"連線資料庫錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void btnRefreshTargetDU_Click(object sender, EventArgs e)
        {
            AppendLog("正在重新載入目標看板...");
            // 根據當前選擇的車站進行篩選
            string selectedStation = cboStation.SelectedItem?.ToString();
            LoadTargetDUFromDatabase(selectedStation);
        }

        private void cboStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 當車站選擇變更時，自動篩選目標看板
            string selectedStation = cboStation.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedStation))
            {
                LoadTargetDUFromDatabase(selectedStation);
            }
        }

        private void cboMessageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 當訊息類型變更時，載入對應的訊息列表
            int messageType = cboMessageType.SelectedIndex;

            if (messageType == 0) // 預錄訊息
            {
                LoadMessageIDsFromDatabase();
            }
            else if (messageType == 1) // 即時訊息
            {
                LoadInstantMessageIDsFromDatabase();
            }
            else
            {
                // 其他類型清空訊息列表
                cboMessageID.Items.Clear();
                cboMessageIDEn.Items.Clear();
            }
        }
        #endregion

        #region Send Methods
        private void SendPreRecordMessage()
        {
            var stationEnum = GetStationEnum(cboStation.Text);
            var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(stationEnum);

            jsonObject.seatID = txtSeatID.Text;

            // 從 ComboBox 取得選中的訊息物件並取得 message_id
            if (cboMessageID.SelectedItem != null)
            {
                var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_pre_record_message;
                if (selectedMessage != null)
                {
                    jsonObject.msg_id = new List<string> { selectedMessage.message_id.ToString() };
                }
                else
                {
                    AppendLog("錯誤：選中的訊息格式不正確");
                    return;
                }
            }
            else
            {
                AppendLog("錯誤：請選擇訊息");
                return;
            }

            // 從 CheckedListBox 取得勾選的目標看板
            var checkedTargets = GetCheckedTargetDU();
            if (checkedTargets.Count == 0)
            {
                AppendLog("錯誤：請至少勾選一個目標看板");
                return;
            }
            jsonObject.target_du = checkedTargets;

            // 設定訊息參數
            jsonObject.message_priority = int.Parse(cboPriority.SelectedItem.ToString());
            jsonObject.move_speed = int.Parse(cboMoveSpeed.SelectedItem.ToString());
            // 從 "0-立即" 格式中提取數字
            string moveModeText = cboMoveMode.SelectedItem.ToString();
            jsonObject.move_mode = int.Parse(moveModeText.Split('-')[0]);

            var message = CreateMessage(jsonObject);
            SendMessage(message, "預錄訊息");
        }

        private void SendInstantMessage()
        {
            var stationEnum = GetStationEnum(cboStation.Text);
            var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(stationEnum);

            jsonObject.seatID = txtSeatID.Text;

            // 從 ComboBox 取得選中的訊息物件並取得 message_id
            if (cboMessageID.SelectedItem != null)
            {
                // 即時訊息使用 dmd_instant_message 類型
                var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_instant_message;
                if (selectedMessage != null)
                {
                    jsonObject.msg_id = selectedMessage.message_id.ToString();
                }
                else
                {
                    AppendLog("錯誤：選中的訊息格式不正確");
                    return;
                }
            }
            else
            {
                AppendLog("錯誤：請選擇訊息");
                return;
            }

            // 從 CheckedListBox 取得勾選的目標看板
            var checkedTargets = GetCheckedTargetDU();
            if (checkedTargets.Count == 0)
            {
                AppendLog("錯誤：請至少勾選一個目標看板");
                return;
            }
            jsonObject.target_du = checkedTargets;

            // 設定訊息參數
            jsonObject.message_priority = int.Parse(cboPriority.SelectedItem.ToString());
            jsonObject.move_speed = int.Parse(cboMoveSpeed.SelectedItem.ToString());
            // 從 "0-立即" 格式中提取數字
            string moveModeText = cboMoveMode.SelectedItem.ToString();
            jsonObject.move_mode = int.Parse(moveModeText.Split('-')[0]);

            var message = CreateMessage(jsonObject);
            SendMessage(message, "即時訊息");
        }

        private void SendPowerTimeSetting()
        {
            var stationEnum = GetStationEnum(cboStation.Text);
            var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting(stationEnum);

            jsonObject.seatID = txtSeatID.Text;
            jsonObject.SqlCommand = ASI.Wanda.DMD.Enum.SqlCommand.update;

            var message = CreateMessage(jsonObject);
            SendMessage(message, "電源設定");
        }

        private void SendTrainMessage()
        {
            var stationEnum = GetStationEnum(cboStation.Text);
            var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMSG(stationEnum);

            // Platform_id 是 int 類型，需要轉換
            if (cboMessageID.SelectedItem != null && int.TryParse(cboMessageID.SelectedItem.ToString(), out int platformId))
            {
                jsonObject.Platform_id = platformId;
            }
            else
            {
                AppendLog("錯誤：請選擇有效的數字訊息ID");
                return;
            }

            jsonObject.Type = "Train";
            jsonObject.Command = "Update";

            var message = CreateMessage(jsonObject);
            SendMessage(message, "列車訊息");
        }

        private void SendScheduleSetting()
        {
            // ScheduleSetting 類別不存在，使用 PowerTimeSetting 代替
            var stationEnum = GetStationEnum(cboStation.Text);
            var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting(stationEnum);

            jsonObject.seatID = txtSeatID.Text;
            jsonObject.SqlCommand = ASI.Wanda.DMD.Enum.SqlCommand.update;

            var message = CreateMessage(jsonObject);
            SendMessage(message, "排程設定 (使用 PowerTimeSetting)");
        }
        #endregion

        #region ListView Events
        private void clbTargetDU_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 防止車站和區域標題被勾選
            if (e.Index >= 0 && e.Index < clbTargetDU.Items.Count)
            {
                ListViewItem item = clbTargetDU.Items[e.Index];

                // 如果是標題行，則取消勾選操作
                if (item.Tag != null && item.Tag.ToString() == "header")
                {
                    e.NewValue = CheckState.Unchecked;
                }
            }
        }
        #endregion

        #region Helper Methods
        private List<string> GetCheckedTargetDU()
        {
            var checkedTargets = new List<string>();

            foreach (ListViewItem item in clbTargetDU.Items)
            {
                // 只處理設備項（非標題行）且已勾選
                if (item.Tag != null && item.Tag.ToString() != "header" && item.Checked)
                {
                    // Tag 中保存了原始設備 ID
                    checkedTargets.Add(item.Tag.ToString());
                }
            }

            return checkedTargets;
        }

        private ASI.Wanda.DMD.Message.Message CreateMessage(object jsonObject)
        {
            string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(jsonObject);
            int messageId = random.Next(1, 100000);

            return new ASI.Wanda.DMD.Message.Message(
                ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                messageId,
                jsonContent
            );
        }

        private void SendMessage(ASI.Wanda.DMD.Message.Message message, string messageTypeName)
        {
            if (mDMD_API == null)
            {
                AppendLog("✗ 尚未連線");
                return;
            }

            int result = mDMD_API.Send(message);
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

            if (result == 0)
            {
                AppendLog($"[{timestamp}] ✓ 發送成功: {messageTypeName}");
                AppendLog($"訊息ID: {message.MessageID}");
                AppendLog($"內容: {message.JsonContent}");
                AppendLog("---");
                ASI.Lib.Log.DebugLog.Log("SendToDCU", $"{messageTypeName}: {message.JsonContent}");
            }
            else
            {
                AppendLog($"[{timestamp}] ✗ 發送失敗，錯誤碼: {result}");
            }
        }

        private ASI.Wanda.DMD.Enum.Station GetStationEnum(string stationText)
        {
            switch (stationText)
            {
                case "OCC": return ASI.Wanda.DMD.Enum.Station.OCC;
                case "LG01": return ASI.Wanda.DMD.Enum.Station.LG01;
                case "LG02": return ASI.Wanda.DMD.Enum.Station.LG02;
                case "LG03": return ASI.Wanda.DMD.Enum.Station.LG03;
                case "LG04": return ASI.Wanda.DMD.Enum.Station.LG04;
                case "LG05": return ASI.Wanda.DMD.Enum.Station.LG05;
                case "LG06": return ASI.Wanda.DMD.Enum.Station.LG06;
                case "LG07": return ASI.Wanda.DMD.Enum.Station.LG07;
                case "LG08": return ASI.Wanda.DMD.Enum.Station.LG08;
                case "LG08A": return ASI.Wanda.DMD.Enum.Station.LG08A;

                default: return ASI.Wanda.DMD.Enum.Station.OCC;
            }
        }

        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((Action)(() => AppendLog(message)));
            }
            else
            {
                txtLog.AppendText($"{message}\r\n");
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        public void CloseConnection()
        {
            if (mDMD_API != null)
            {
                mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                mDMD_API.Dispose();
            }
        }
        #endregion

        #region Instant Message Methods
        private void btnLoadInstantMessage_Click(object sender, EventArgs e)
        {
            try
            {
                // 確認當前訊息類型是否為即時訊息
                if (cboMessageType.SelectedIndex != 1)
                {
                    AppendLog("請先選擇「即時訊息 (SendInstantMessage)」類型");
                    return;
                }

                // 檢查是否有選擇訊息
                if (cboMessageID.SelectedItem == null)
                {
                    AppendLog("請先選擇一個即時訊息");
                    return;
                }

                // 從 ComboBox 取得選中的訊息 ID
                string selectedMessageId = null;

                // 根據當前資料來源判斷類型
                if (cboMessageID.SelectedItem is ASI.Wanda.DMD.DB.Models.dmd_instant_message)
                {
                    var instantMsg = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_instant_message;
                    selectedMessageId = instantMsg.message_id.ToString();
                }
                else if (cboMessageID.SelectedItem is ASI.Wanda.DMD.DB.Models.dmd_pre_record_message)
                {
                    var preRecordMsg = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_pre_record_message;
                    selectedMessageId = preRecordMsg.message_id.ToString();
                }
                else
                {
                    AppendLog("錯誤：無法識別的訊息類型");
                    return;
                }

                if (string.IsNullOrEmpty(selectedMessageId))
                {
                    AppendLog("錯誤：無法取得訊息 ID");
                    return;
                }

                // 從資料庫載入即時訊息
                var instantMessages = ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.SelectAll();
                currentInstantMessage = instantMessages.FirstOrDefault(m => m.message_id.ToString() == selectedMessageId);

                if (currentInstantMessage != null)
                {
                    // 顯示訊息內容到 TextBox
                    txtInstantMessageCHN.Text = currentInstantMessage.message_content ?? "";
                    txtInstantMessageENG.Text = currentInstantMessage.message_content_en ?? "";
                    AppendLog($"✓ 已載入即時訊息 ID: {selectedMessageId}");
                }
                else
                {
                    AppendLog($"⚠ 找不到訊息 ID: {selectedMessageId}");
                    txtInstantMessageCHN.Text = "";
                    txtInstantMessageENG.Text = "";
                }
            }
            catch (Exception ex)
            {
                AppendLog($"載入即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void btnSaveInstantMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentInstantMessage == null)
                {
                    AppendLog("錯誤：請先載入一個即時訊息");
                    return;
                }

                // 更新訊息內容
                currentInstantMessage.message_content = txtInstantMessageCHN.Text;
                currentInstantMessage.message_content_en = txtInstantMessageENG.Text;

                // 更新資料庫
                ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.UpdateInstantMessages(
                    currentInstantMessage.message_id,
                    currentInstantMessage.message_type,
                    currentInstantMessage.message_priority,
                    currentInstantMessage.move_mode,
                    currentInstantMessage.move_speed,
                    currentInstantMessage.Interval,
                    currentInstantMessage.message_content,
                    currentInstantMessage.font_type,
                    currentInstantMessage.font_size,
                    currentInstantMessage.font_color,
                    currentInstantMessage.message_content_en,
                    currentInstantMessage.font_type_en,
                    currentInstantMessage.font_size_en,
                    currentInstantMessage.font_color_en
                );

                AppendLog($"✓ 即時訊息已更新 ID: {currentInstantMessage.message_id}");
                AppendLog($"  中文: {currentInstantMessage.message_content}");
                AppendLog($"  英文: {currentInstantMessage.message_content_en}");

                // 重新載入訊息列表以更新顯示
                LoadInstantMessageIDsFromDatabase();
            }
            catch (Exception ex)
            {
                AppendLog($"保存即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void LoadInstantMessageIDsFromDatabase()
        {
            try
            {
                // 從資料庫讀取所有即時訊息
                var messages = ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.SelectAll();

                if (messages != null && messages.Count > 0)
                {
                    // 清空現有項目
                    cboMessageID.Items.Clear();
                    cboMessageIDEn.Items.Clear();

                    // 設定 ComboBox 的顯示成員
                    cboMessageID.DisplayMember = "message_content";
                    cboMessageID.ValueMember = "message_id";
                    cboMessageIDEn.DisplayMember = "message_content_en";
                    cboMessageIDEn.ValueMember = "message_id";

                    // 添加所有 message 物件到兩個下拉選單
                    foreach (var message in messages)
                    {
                        cboMessageID.Items.Add(message);
                        cboMessageIDEn.Items.Add(message);
                    }

                    // 預設選擇第一個
                    if (cboMessageID.Items.Count > 0)
                    {
                        cboMessageID.SelectedIndex = 0;
                        cboMessageIDEn.SelectedIndex = 0;
                    }

                    AppendLog($"✓ 已載入 {cboMessageID.Items.Count} 個即時訊息");
                }
                else
                {
                    // 沒有訊息時，清空下拉選單
                    cboMessageID.Items.Clear();
                    cboMessageIDEn.Items.Clear();
                    AppendLog("⚠ 資料庫中沒有即時訊息");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"載入即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private bool isUpdatingSelection = false; // 防止循環觸發

        private void cboMessageID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSelection && cboMessageID.SelectedIndex >= 0)
            {
                isUpdatingSelection = true;
                if (cboMessageIDEn.Items.Count > cboMessageID.SelectedIndex)
                {
                    cboMessageIDEn.SelectedIndex = cboMessageID.SelectedIndex;
                }
                isUpdatingSelection = false;

                // 載入訊息的參數值到 UI 控制項
                LoadMessageParameters();
            }
        }

        private void cboMessageIDEn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSelection && cboMessageIDEn.SelectedIndex >= 0)
            {
                isUpdatingSelection = true;
                if (cboMessageID.Items.Count > cboMessageIDEn.SelectedIndex)
                {
                    cboMessageID.SelectedIndex = cboMessageIDEn.SelectedIndex;
                }
                isUpdatingSelection = false;

                // 載入訊息的參數值到 UI 控制項
                LoadMessageParameters();
            }
        }

        /// <summary>
        /// 從選中的訊息載入參數值到 UI 控制項
        /// </summary>
        private void LoadMessageParameters()
        {
            try
            {
                if (cboMessageID.SelectedItem == null)
                    return;

                // 根據訊息類型讀取對應的參數
                if (cboMessageType.SelectedIndex == 0) // 預錄訊息
                {
                    var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_pre_record_message;
                    if (selectedMessage != null)
                    {
                        // 設定優先等級 (1-5)
                        if (selectedMessage.message_priority >= 1 && selectedMessage.message_priority <= 5)
                        {
                            cboPriority.SelectedIndex = selectedMessage.message_priority - 1;
                        }

                        // 設定移動速度 (1-5)
                        if (selectedMessage.move_speed >= 1 && selectedMessage.move_speed <= 5)
                        {
                            cboMoveSpeed.SelectedIndex = selectedMessage.move_speed - 1;
                        }

                        // 設定移動方式 (0-6)
                        if (selectedMessage.move_mode >= 0 && selectedMessage.move_mode <= 6)
                        {
                            cboMoveMode.SelectedIndex = selectedMessage.move_mode;
                        }
                    }
                }
                else if (cboMessageType.SelectedIndex == 1) // 即時訊息
                {
                    var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_instant_message;
                    if (selectedMessage != null)
                    {
                        // 設定優先等級 (1-5)
                        if (selectedMessage.message_priority >= 1 && selectedMessage.message_priority <= 5)
                        {
                            cboPriority.SelectedIndex = selectedMessage.message_priority - 1;
                        }

                        // 設定移動速度 (1-5)
                        if (selectedMessage.move_speed >= 1 && selectedMessage.move_speed <= 5)
                        {
                            cboMoveSpeed.SelectedIndex = selectedMessage.move_speed - 1;
                        }

                        // 設定移動方式 (0-6)
                        if (selectedMessage.move_mode >= 0 && selectedMessage.move_mode <= 6)
                        {
                            cboMoveMode.SelectedIndex = selectedMessage.move_mode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"載入訊息參數錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }
        #endregion
    }
}
