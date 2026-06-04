using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DMDService.Services.Interfaces;
using DMDService.Services.Models;

namespace UITest
{
    public partial class TaskCMFT : UserControl
    {
        #region Fields
        private readonly ICmftMessageService _cmftService;
        #endregion

        #region Constructor
        public TaskCMFT()
        {
            InitializeComponent();
        }

        public TaskCMFT(ICmftMessageService cmftService) : this()
        {
            _cmftService = cmftService ?? throw new ArgumentNullException(nameof(cmftService));

            // Subscribe to service events for UI updates
            _cmftService.LogConnectionState += text => UpdateTextSafe(connetText, "連線狀態" + text);
            _cmftService.LogSendData += text => UpdateTextSafe(sendDataText, "傳送內容 : " + text);
            _cmftService.LogResponseData += text => UpdateTextSafe(resText, "回覆內容 : " + text);
            _cmftService.LogReceiveData += text => UpdateTextSafe(ReceDataTextDCU, text);
            _cmftService.LogBlackList += text => UpdateTextSafe(blackListText, "目前的狀態異常設備 : " + text);
            _cmftService.LogBlackListSeparator += text => UpdateTextSafe(blackListText, text);

            // Initialize DB and load data
            InitializeDatabaseAndLoadData();
        }
        #endregion

        #region Initialization
        private void InitializeDatabaseAndLoadData()
        {
            if (_cmftService == null) return;

            try
            {
                _cmftService.InitializeDatabases(
                    "127.0.0.1", "5432", "DMDDB", "postgres", "postgres",
                    "127.0.0.1", "5432", "CMFTDB", "postgres", "postgres");

                LoadEquipStatusGrid();
                LoadPanelStatusGrid();

                _cmftService.StartTimers();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("TaskCMFT", ex);
            }
        }
        #endregion

        #region Data Grid Loading
        private void LoadEquipStatusGrid()
        {
            if (_cmftService == null) return;

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("equip_id", "Equip ID");
            dataGridView1.Columns.Add("equip_status", "Equip Status");

            var statuses = _cmftService.GetEquipStatuses();
            foreach (var status in statuses)
            {
                dataGridView1.Rows.Add(status.EquipId, status.EquipStatus);
            }
        }

        private void LoadPanelStatusGrid()
        {
            if (_cmftService == null) return;

            dataGridViewPanel.Columns.Clear();
            dataGridViewPanel.Rows.Clear();
            dataGridViewPanel.Columns.Add("station_id", "車站");
            dataGridViewPanel.Columns.Add("area_id", "地區");
            dataGridViewPanel.Columns.Add("device_id", "裝置_id");
            dataGridViewPanel.Columns.Add("device_status", "設備狀態");

            var panels = _cmftService.GetTargetPanels();
            foreach (var panel in panels)
            {
                dataGridViewPanel.Rows.Add(panel.StationId, panel.AreaId, panel.DeviceId, panel.DeviceStatus);
            }
        }
        #endregion

        #region Button Events
        private void btnRefreshEquip_Click(object sender, EventArgs e)
        {
            LoadEquipStatusGrid();
        }

        private void btnRefreshPanel_Click(object sender, EventArgs e)
        {
            LoadPanelStatusGrid();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_cmftService == null) return;

            int result = _cmftService.Connect(textBoxConnIP.Text, textBoxConnPort.Text, textBoxType.Text);
            if (result != 0)
            {
                string message = GetConnectionResultMessage(result);
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("成功開啟");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (_cmftService == null) return;

            _cmftService.InitializeCmftDatabase("127.0.0.1", "5432", "CMFTDB", "postgres", "postgres");
            _cmftService.SyncPlayList();
            _cmftService.SyncPreRecordMessages();
        }
        #endregion

        #region Helper Methods
        private void UpdateTextSafe(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke((Action)(() => UpdateTextSafe(control, text)));
            }
            else
            {
                if (control is RichTextBox rtb)
                    rtb.Text += text + "\r\n";
            }
        }

        private string GetConnectionResultMessage(int result)
        {
            switch (result)
            {
                case 0: return "成功開啟";
                case -1: return "例外錯誤";
                case -2: return "未成功開啟";
                case -3: return "剖析連線字串發生錯誤";
                case -4: return "初始化 Socket 相關屬性發生錯誤";
                case -5: return "關閉所有 Sockets 時發生錯誤";
                case -6: return "Socket server 無法正常繫結通訊埠";
                default: return "未知的錯誤";
            }
        }

        public void CloseConnection()
        {
            _cmftService?.StopTimers();
            _cmftService?.Disconnect();
        }
        #endregion
    }
}
