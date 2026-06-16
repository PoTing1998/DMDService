using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DMDService.Services.Interfaces;
using DMDService.Services.Models;

namespace UITest
{
    public partial class SendToDCU : UserControl
    {
        #region Fields
        private readonly IDmdMessageService _messageService;
        private ASI.Wanda.DMD.DB.Models.dmd_instant_message currentInstantMessage = null;
        #endregion

        #region Constructor
        public SendToDCU()
        {
            InitializeComponent();
            InitializeUI();
        }

        public SendToDCU(IDmdMessageService messageService) : this()
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _messageService.LogMessage += AppendLog;
        }
        #endregion

        #region Initialize
        private void InitializeUI()
        {
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8001";
            cboType.Items.AddRange(new object[] { "Server", "Client" });
            cboType.SelectedIndex = 0;

            cboMessageType.Items.AddRange(new object[] {
                "預錄訊息 (SendPreRecordMessage)",
                "即時訊息 (SendInstantMessage)"
            });
            cboMessageType.SelectedIndex = 0;
            cboMessageType.SelectedIndexChanged += cboMessageType_SelectedIndexChanged;

            cboStation.Items.AddRange(new object[] {
                "全部", "LG01", "LG02", "LG03", "LG04", "LG05",
                "LG06", "LG07", "LG08", "LG08A"
            });
            cboStation.SelectedIndex = 0;

            cboArea.Items.Add("全部");
            cboArea.SelectedIndex = 0;

            cboPriority.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cboPriority.SelectedIndex = 2;

            cboMoveSpeed.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cboMoveSpeed.SelectedIndex = 2;

            cboMoveMode.Items.AddRange(new object[] {
                "0-立即", "1-靜態", "2-上移", "3-下移", "4-左移", "5-右移", "6-閃爍"
            });
            cboMoveMode.SelectedIndex = 0;

            txtLog.Text = "=== 發送至 DCU 日誌 ===\r\n";
            AppendLog("提示：請先點擊「連線資料庫」載入訊息");

            InitializeListViewColumns();
        }

        private void InitializeListViewColumns()
        {
            clbTargetDU.Columns.Clear();
            clbTargetDU.Columns.Add("車站", 80);
            clbTargetDU.Columns.Add("區域", 80);
            clbTargetDU.Columns.Add("設備ID", 200);
        }
        #endregion

        #region Button Events
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_messageService == null) return;

            int result = _messageService.Connect(txtIP.Text, txtPort.Text, cboType.Text);

            if (result == 0)
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnSend.Enabled = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_messageService == null) return;

            _messageService.Disconnect();
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
        }

        private void btnConnectDB_Click(object sender, EventArgs e)
        {
            if (_messageService == null) return;

            if (string.IsNullOrWhiteSpace(txtDBIP.Text))
            { AppendLog("錯誤：請輸入資料庫 IP"); return; }
            if (string.IsNullOrWhiteSpace(txtDBPort.Text))
            { AppendLog("錯誤：請輸入資料庫 Port"); return; }
            if (string.IsNullOrWhiteSpace(txtDBName.Text))
            { AppendLog("錯誤：請輸入資料庫名稱"); return; }
            if (string.IsNullOrWhiteSpace(txtDBUserID.Text))
            { AppendLog("錯誤：請輸入使用者帳號"); return; }

            try
            {
                AppendLog($"正在連線至資料庫 {txtDBIP.Text}:{txtDBPort.Text}/{txtDBName.Text}...");

                bool success = _messageService.InitializeDatabase(
                    txtDBIP.Text, txtDBPort.Text, txtDBName.Text,
                    txtDBUserID.Text, txtDBPassword.Text);

                if (success)
                {
                    LoadPreRecordMessages();
                    string selectedStation = cboStation.SelectedItem?.ToString();
                    PopulateAreaFilter(selectedStation);
                    LoadTargetDevices(selectedStation, "全部");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"連線資料庫錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_messageService == null) return;

            try
            {
                int messageType = cboMessageType.SelectedIndex;
                switch (messageType)
                {
                    case 0: DoSendPreRecordMessage(); break;
                    case 1: DoSendInstantMessage(); break;
                    default: AppendLog("請選擇訊息類型"); break;
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
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|Excel CSV (*.csv)|*.csv|所有文件 (*.*)|*.*";
                    saveFileDialog.Title = "匯出 DCU 日誌";
                    saveFileDialog.FileName = $"DCU_Log_{DateTime.Now:yyyyMMdd_HHmmss}";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                            ExportLogToCsv(filePath);
                        else
                            System.IO.File.WriteAllText(filePath, txtLog.Text, Encoding.UTF8);

                        AppendLog($"✓ 日誌已匯出至: {filePath}");
                        MessageBox.Show($"日誌已成功匯出至:\n{filePath}", "匯出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (cboMessageType.SelectedIndex == 0)
                LoadPreRecordMessages();
            else if (cboMessageType.SelectedIndex == 1)
                LoadInstantMessages();
        }

        private void btnRefreshTargetDU_Click(object sender, EventArgs e)
        {
            AppendLog("正在重新載入目標看板...");
            string selectedStation = cboStation.SelectedItem?.ToString();
            string selectedArea = cboArea.SelectedItem?.ToString();
            LoadTargetDevices(selectedStation, selectedArea);
        }

        private void cboStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStation = cboStation.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedStation))
            {
                PopulateAreaFilter(selectedStation);
                LoadTargetDevices(selectedStation, "全部");
            }
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStation = cboStation.SelectedItem?.ToString();
            string selectedArea = cboArea.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedStation))
                LoadTargetDevices(selectedStation, selectedArea);
        }

        private void cboMessageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int messageType = cboMessageType.SelectedIndex;
            if (messageType == 0)
                LoadPreRecordMessages();
            else if (messageType == 1)
                LoadInstantMessages();
            else
            {
                cboMessageID.Items.Clear();
                cboMessageIDEn.Items.Clear();
            }
        }
        #endregion

        #region Send Methods
        private void DoSendPreRecordMessage()
        {
            if (cboMessageID.SelectedItem == null)
            { AppendLog("錯誤：請選擇訊息"); return; }

            var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_pre_record_message;
            if (selectedMessage == null)
            { AppendLog("錯誤：選中的訊息格式不正確"); return; }

            var checkedTargets = GetCheckedTargetDU();
            if (checkedTargets.Count == 0)
            { AppendLog("錯誤：請至少勾選一個目標看板"); return; }

            string moveModeText = cboMoveMode.SelectedItem.ToString();
            int moveMode = int.Parse(moveModeText.Split('-')[0]);

            _messageService.SendPreRecordMessage(
                txtSeatID.Text, cboStation.Text,
                selectedMessage.message_id.ToString(),
                checkedTargets,
                int.Parse(cboPriority.SelectedItem.ToString()),
                int.Parse(cboMoveSpeed.SelectedItem.ToString()),
                moveMode);
        }

        private void DoSendInstantMessage()
        {
            if (cboMessageID.SelectedItem == null)
            { AppendLog("錯誤：請選擇訊息"); return; }

            var selectedMessage = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_instant_message;
            if (selectedMessage == null)
            { AppendLog("錯誤：選中的訊息格式不正確"); return; }

            var checkedTargets = GetCheckedTargetDU();
            if (checkedTargets.Count == 0)
            { AppendLog("錯誤：請至少勾選一個目標看板"); return; }

            string moveModeText = cboMoveMode.SelectedItem.ToString();
            int moveMode = int.Parse(moveModeText.Split('-')[0]);

            _messageService.SendInstantMessage(
                txtSeatID.Text, cboStation.Text,
                selectedMessage.message_id.ToString(),
                checkedTargets,
                int.Parse(cboPriority.SelectedItem.ToString()),
                int.Parse(cboMoveSpeed.SelectedItem.ToString()),
                moveMode);
        }
        #endregion

        #region Data Loading (UI population from Service)
        private void LoadPreRecordMessages()
        {
            if (_messageService == null) return;

            var messages = _messageService.GetPreRecordMessages();

            cboMessageID.Items.Clear();
            cboMessageIDEn.Items.Clear();
            cboMessageID.DisplayMember = "message_content";
            cboMessageID.ValueMember = "message_id";
            cboMessageIDEn.DisplayMember = "message_content_en";
            cboMessageIDEn.ValueMember = "message_id";

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    cboMessageID.Items.Add(message);
                    cboMessageIDEn.Items.Add(message);
                }
                cboMessageID.SelectedIndex = 0;
                cboMessageIDEn.SelectedIndex = 0;
                AppendLog($"✓ 已載入 {messages.Count} 個訊息");
            }
            else
            {
                AppendLog("⚠ 資料庫中沒有預錄訊息");
            }
        }

        private void LoadInstantMessages()
        {
            if (_messageService == null) return;

            var messages = _messageService.GetInstantMessages();

            cboMessageID.Items.Clear();
            cboMessageIDEn.Items.Clear();
            cboMessageID.DisplayMember = "message_content";
            cboMessageID.ValueMember = "message_id";
            cboMessageIDEn.DisplayMember = "message_content_en";
            cboMessageIDEn.ValueMember = "message_id";

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    cboMessageID.Items.Add(message);
                    cboMessageIDEn.Items.Add(message);
                }
                cboMessageID.SelectedIndex = 0;
                cboMessageIDEn.SelectedIndex = 0;
                AppendLog($"✓ 已載入 {messages.Count} 個即時訊息");
            }
            else
            {
                AppendLog("⚠ 資料庫中沒有即時訊息");
            }
        }

        private void LoadTargetDevices(string stationFilter, string areaFilter = null)
        {
            if (_messageService == null) return;

            var devices = _messageService.GetTargetDevices(stationFilter);

            // 依區域篩選
            if (!string.IsNullOrEmpty(areaFilter) && areaFilter != "全部")
                devices = devices.Where(d => d.AreaId == areaFilter).ToList();

            clbTargetDU.Items.Clear();

            foreach (var device in devices)
            {
                var item = new ListViewItem(device.StationId);
                item.SubItems.Add(device.AreaId);
                item.SubItems.Add(device.ToTargetString());
                item.Tag = device.ToTargetString();
                item.Checked = true;
                clbTargetDU.Items.Add(item);
            }

            string log = $"✓ 已載入 {devices.Count} 個目標看板";
            if (!string.IsNullOrEmpty(stationFilter) && stationFilter != "全部")
                log += $" (車站: {stationFilter}";
            if (!string.IsNullOrEmpty(areaFilter) && areaFilter != "全部")
                log += $", 區域: {areaFilter}";
            if (log.Contains("(")) log += ")";
            AppendLog(log);
        }

        private void PopulateAreaFilter(string stationFilter)
        {
            if (_messageService == null) return;

            var devices = _messageService.GetTargetDevices(stationFilter);
            var areas = devices.Select(d => d.AreaId)
                               .Where(a => !string.IsNullOrEmpty(a))
                               .Distinct()
                               .OrderBy(a => a)
                               .ToList();

            cboArea.SelectedIndexChanged -= cboArea_SelectedIndexChanged;
            cboArea.Items.Clear();
            cboArea.Items.Add("全部");
            foreach (var area in areas)
                cboArea.Items.Add(area);
            cboArea.SelectedIndex = 0;
            cboArea.SelectedIndexChanged += cboArea_SelectedIndexChanged;
        }
        #endregion

        #region ListView Events
        private void clbTargetDU_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index >= 0 && e.Index < clbTargetDU.Items.Count)
            {
                ListViewItem item = clbTargetDU.Items[e.Index];
                if (item.Tag != null && item.Tag.ToString() == "header")
                    e.NewValue = CheckState.Unchecked;
            }
        }
        #endregion

        #region Helper Methods
        private List<string> GetCheckedTargetDU()
        {
            var checkedTargets = new List<string>();
            foreach (ListViewItem item in clbTargetDU.Items)
            {
                if (item.Tag != null && item.Tag.ToString() != "header" && item.Checked)
                    checkedTargets.Add(item.Tag.ToString());
            }
            return checkedTargets;
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

        private void ExportLogToCsv(string filePath)
        {
            var lines = txtLog.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            sb.AppendLine("行號,時間,內容");

            int lineNum = 1;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string time = "";
                string content = line;

                int bracketStart = line.IndexOf('[');
                int bracketEnd = line.IndexOf(']');
                if (bracketStart >= 0 && bracketEnd > bracketStart)
                {
                    time = line.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
                    content = line.Substring(bracketEnd + 1).TrimStart();
                }

                string escapedContent = content.Replace("\"", "\"\"");
                sb.AppendLine($"{lineNum},\"{time}\",\"{escapedContent}\"");
                lineNum++;
            }

            System.IO.File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        }

        public void CloseConnection()
        {
            _messageService?.Disconnect();
        }
        #endregion

        #region Instant Message Methods
        private void btnLoadInstantMessage_Click(object sender, EventArgs e)
        {
            if (_messageService == null) return;

            try
            {
                if (cboMessageType.SelectedIndex != 1)
                { AppendLog("請先選擇「即時訊息 (SendInstantMessage)」類型"); return; }

                if (cboMessageID.SelectedItem == null)
                { AppendLog("請先選擇一個即時訊息"); return; }

                string selectedMessageId = null;
                if (cboMessageID.SelectedItem is ASI.Wanda.DMD.DB.Models.dmd_instant_message instantMsg)
                    selectedMessageId = instantMsg.message_id.ToString();
                else if (cboMessageID.SelectedItem is ASI.Wanda.DMD.DB.Models.dmd_pre_record_message preRecordMsg)
                    selectedMessageId = preRecordMsg.message_id.ToString();

                if (string.IsNullOrEmpty(selectedMessageId))
                { AppendLog("錯誤：無法取得訊息 ID"); return; }

                currentInstantMessage = _messageService.GetInstantMessage(selectedMessageId);

                if (currentInstantMessage != null)
                {
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
            if (_messageService == null) return;

            try
            {
                if (currentInstantMessage == null)
                { AppendLog("錯誤：請先載入一個即時訊息"); return; }

                currentInstantMessage.message_content = txtInstantMessageCHN.Text;
                currentInstantMessage.message_content_en = txtInstantMessageENG.Text;

                bool success = _messageService.SaveInstantMessage(currentInstantMessage);

                if (success)
                {
                    AppendLog($"  中文: {currentInstantMessage.message_content}");
                    AppendLog($"  英文: {currentInstantMessage.message_content_en}");
                    LoadInstantMessages();
                }
            }
            catch (Exception ex)
            {
                AppendLog($"保存即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("SendToDCU", ex);
            }
        }

        private bool isUpdatingSelection = false;

        private void cboMessageID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSelection && cboMessageID.SelectedIndex >= 0)
            {
                isUpdatingSelection = true;
                if (cboMessageIDEn.Items.Count > cboMessageID.SelectedIndex)
                    cboMessageIDEn.SelectedIndex = cboMessageID.SelectedIndex;
                isUpdatingSelection = false;
                LoadMessageParameters();
            }
        }

        private void cboMessageIDEn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdatingSelection && cboMessageIDEn.SelectedIndex >= 0)
            {
                isUpdatingSelection = true;
                if (cboMessageID.Items.Count > cboMessageIDEn.SelectedIndex)
                    cboMessageID.SelectedIndex = cboMessageIDEn.SelectedIndex;
                isUpdatingSelection = false;
                LoadMessageParameters();
            }
        }

        private void LoadMessageParameters()
        {
            if (cboMessageID.SelectedItem == null) return;

            try
            {
                int priority = -1, moveSpeed = -1, moveMode = -1;

                if (cboMessageType.SelectedIndex == 0)
                {
                    var msg = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_pre_record_message;
                    if (msg != null)
                    { priority = msg.message_priority; moveSpeed = msg.move_speed; moveMode = msg.move_mode; }
                }
                else if (cboMessageType.SelectedIndex == 1)
                {
                    var msg = cboMessageID.SelectedItem as ASI.Wanda.DMD.DB.Models.dmd_instant_message;
                    if (msg != null)
                    { priority = msg.message_priority; moveSpeed = msg.move_speed; moveMode = msg.move_mode; }
                }

                if (priority >= 1 && priority <= 5) cboPriority.SelectedIndex = priority - 1;
                if (moveSpeed >= 1 && moveSpeed <= 5) cboMoveSpeed.SelectedIndex = moveSpeed - 1;
                if (moveMode >= 0 && moveMode <= 6) cboMoveMode.SelectedIndex = moveMode;
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
