using ASI.Wanda.CMFT;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace UITest
{
    public partial class TaskCMFT : UserControl
    {
        #region Fields
        public static ASI.Wanda.CMFT.CMFT_API CMFT_API = new CMFT_API();
        private System.Timers.Timer updateBlackListTimer;
        private static System.Timers.Timer sendMessageTimer;
        private List<string> failedDeviceList = new List<string>();
        #endregion
        #region Constructor
        public TaskCMFT()
        {
            InitializeComponent();
            InitializeDatabaseConnections();
            InitializeTimers();
            LoadDateGridViewPanel();
            LoadDataGridViewData();
            
            // 載入 DataGridView 資料

            LoadDateGridViewPanel();
            //設定黑名單的定時器
            updateBlackListTimer = new System.Timers.Timer();
            updateBlackListTimer.Interval = 6000;
            updateBlackListTimer.Elapsed += UpdateBlackListTimer_Elapsed; // 設置事件處理程序
            updateBlackListTimer.Start();
            //  InitializeConnection();
            LoadDataGridViewData();
            //發送訊息
            sendMessageTimer = new System.Timers.Timer(7000);
            sendMessageTimer.Elapsed += SendMessage;
            sendMessageTimer.AutoReset = true;
            sendMessageTimer.Enabled = true;
        }

        public class DeviceInfo
        {
            public string StationID { get; set; }
            public string AreaID { get; set; }
            public string DeviceID { get; set; }
        }
        #endregion
        #region Methods
        private void InitializeDatabaseConnections()
        {
            ASI.Wanda.DMD.DB.Manager.Initializer("127.0.0.1", "5432", "DMDDB", "postgres", "postgres", "DMDServer");
            ASI.Wanda.CMFT.DB.Manager.Initializer("127.0.0.1", "5432", "CMFTDB", "postgres", "postgres", "admin");
        }
        private void InitializeTimers()
        {
            updateBlackListTimer = new System.Timers.Timer(6000);
            updateBlackListTimer.Elapsed += UpdateBlackListTimer_Elapsed;
            updateBlackListTimer.Start();

            sendMessageTimer = new System.Timers.Timer(7000);
            sendMessageTimer.Elapsed += SendMessage;
            sendMessageTimer.AutoReset = true;
            sendMessageTimer.Enabled = true;
        }
        private void InitializeConnection()
        {
            string sConn = $"IP={textBoxConnIP.Text};Port={textBoxConnPort.Text};Type={textBoxType.Text}";
            if (CMFT_API != null)
            {
                CMFT_API.ReceivedEvent -= CMFT_API_ReceivedEvent;
                CMFT_API.Dispose();
            }
            var result = CMFT_API.Initial(sConn, "CMFT");
            HandleConnectionResult(result);
            CMFT_API.ReceivedEvent += CMFT_API_ReceivedEvent;
        }
        private void HandleConnectionResult(int result)
        {
            string message;
            switch (result)
            {
                case 0:
                    message = "成功開啟";
                    break;
                case -1:
                    message = "例外錯誤";
                    break;
                case -2:
                    message = "未成功開啟";
                    break;
                case -3:
                    message = "剖析連線字串發生錯誤";
                    break;
                case -4:
                    message = "初始化 Socket 相關屬性發生錯誤";
                    break;
                case -5:
                    message = "關閉所有 Sockets 時發生錯誤";
                    break;
                case -6:
                    message = "Socket server 無法正常繫結通訊埠";
                    break;
                default:
                    message = "未知的錯誤";
                    break;
            }
            MessageBox.Show(message);
        }

        private void UpdateBlackListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateBlackListFromDatabase(); // 更新黑名單的方式   
        }
  


        #region event
        /// <summary>
        /// 從CMFT 接收訊息
        /// </summary>
        /// <param name="DMDServerMessage"></param>
        private void CMFT_API_ReceivedEvent(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {


            string sLog = "";
            try
            {

                string sRcvTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
                string sByteArray = ASI.Lib.Text.Parsing.String.BytesToHexString(CMFTServerMessage.CompleteContent, "");
                if (CMFTServerMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Heartbeat)
                {
                    string sJsonData = CMFTServerMessage.JsonContent;
                    string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "JsonObjectName");
                    int iMsgID = CMFTServerMessage.MessageID;
                    var temp = PrintJsonObject(CMFTServerMessage);
                    UpdateReceDataText(string.Format("{0} \r\n", temp));
                }
                else if (CMFTServerMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Ack)
                {
                    ///Ack
                    sLog = string.Format("Ack，訊息識別碼:[{0}]", CMFTServerMessage.MessageID);
                    UpdateReceDataText($"{sRcvTime} {sLog} \r\n");
                    ASI.Wanda.CMFT.Message.Message omessage = new ASI.Wanda.CMFT.Message.Message();
                    omessage.MessageID = CMFTServerMessage.MessageID;
                    omessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Ack;
                    //  CMFT_API.Send(omessage);

                }
                else if (CMFTServerMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Command)
                {
                    string sJsonData = CMFTServerMessage.JsonContent;
                    string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "JsonObjectName");
                    int iMsgID = CMFTServerMessage.MessageID;
                    sLog = $"從CMFT Server收到:{sByteArray}；訊息類別碼:{CMFTServerMessage.MessageType}；識別碼:{iMsgID}；長度:{CMFTServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
                    ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");
                    ///Change/Command
                    sLog = string.Format("JsonObjectName = {0}\r\n", sJsonObjectName);
                    var oObject = ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);
                    /// CMFT TO DMD (2).	預錄訊息命令 
                    if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage")
                    {
                        var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);
                        //失敗的看板
                        var failedList = new List<string>();

                        var stationsDuDictionary = new Dictionary<string, List<string>>();
                        ///整理成車站加上看板
                        foreach (var Targetdu in oJsonObject.target_du)
                        {
                            var station = Targetdu.Split('_')[0];
                            if (!stationsDuDictionary.ContainsKey(station))
                            {
                                stationsDuDictionary[station] = new List<string>();
                            }
                            stationsDuDictionary[station].Add(Targetdu);
                        }
                        ///要傳送的目標看板 車站 看板
                        var deviceInfoList = oJsonObject.target_du
                            .Where(Targetdu => Targetdu.Split('_').Length >= 3)
                            .Select(Targetdu =>
                            {
                                var parts = Targetdu.Split('_');
                                return new DeviceInfo
                                {
                                    StationID = parts[0],
                                    AreaID = parts[1],
                                    DeviceID = parts[2]
                                };
                            })
                            .ToList();
                        ///尋找有誤的看板
                        var PanelError = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectPanelStatusError();
                        /// 比對是否有失效的看板
                        foreach (var deviceInfo in deviceInfoList)
                        {
                            string deviceInfoString = $"{deviceInfo.StationID}_{deviceInfo.AreaID}_{deviceInfo.DeviceID}";
                            bool isDeviceInfoDuplicate = PanelError.Any(target => target.station_id == deviceInfo.StationID && target.area_id == deviceInfo.AreaID && target.device_id == deviceInfo.DeviceID);

                            if (isDeviceInfoDuplicate)
                            {
                                failedList.Add(deviceInfoString);
                            }
                        }

                        ///依照車站的數量回復 並且判斷成功與否
                        foreach (var station in stationsDuDictionary)
                        {
                            if (PanelError.Any(target => target.station_id == station.Key))
                            {
                                SendResponsePreRecordMessage(CMFTServerMessage, oJsonObject.SeatID, oJsonObject.msg_id, station.Key, false, failedDeviceList.Contains(station.Key) ? station.Value : failedList);
                            }
                            else
                            {
                                SendResponsePreRecordMessage(CMFTServerMessage, oJsonObject.SeatID, oJsonObject.msg_id, station.Key, true, failedDeviceList.Contains(station.Key) ? station.Value : new List<string>());
                            }
                        }
                    }

                    /// CMFT TO DMD (3).	即時訊息命令 
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage")
                    {
                        var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);

                        //失敗看板 
                        var failedList = new List<string>();

                        var stationsDuDictionary = new Dictionary<string, List<string>>();

                        foreach (var Targetdu in oJsonObject.target_du)
                        {
                            var station = Targetdu.Split('_')[0];
                            if (!stationsDuDictionary.ContainsKey(station))
                            {
                                stationsDuDictionary[station] = new List<string>();
                            }
                            stationsDuDictionary[station].Add(Targetdu);
                        }
                        ///要傳送的目標看板 車站 看板
                        var deviceInfoList = oJsonObject.target_du
                            .Where(Targetdu => Targetdu.Split('_').Length >= 3)
                            .Select(Targetdu =>
                            {
                                var parts = Targetdu.Split('_');
                                return new DeviceInfo
                                {
                                    StationID = parts[0],
                                    AreaID = parts[1],
                                    DeviceID = parts[2]
                                };
                            })
                            .ToList();
                        //找尋 狀態異常的看板 並加入倒list中
                        var PanelError = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectPanelStatusError();
                        foreach (var deviceInfo in deviceInfoList)
                        {
                            string deviceInfoString = $"{deviceInfo.StationID}_{deviceInfo.AreaID}_{deviceInfo.DeviceID}";
                            bool isDeviceInfoDuplicate = PanelError.Any(target => target.station_id == deviceInfo.StationID && target.area_id == deviceInfo.AreaID && target.device_id == deviceInfo.DeviceID);
                            if (isDeviceInfoDuplicate)
                            {
                                failedList.Add(deviceInfoString);
                            }
                        }
                        //回應報面板的狀態
                        foreach (var station in stationsDuDictionary)
                        {
                            if (PanelError.Any(target => target.station_id == station.Key))
                            {
                                sendResponInstantMSG(CMFTServerMessage, oJsonObject.SeatID, oJsonObject.msg_id, station.Key, false, failedDeviceList.Contains(station.Key) ? station.Value : failedList);
                            }
                            else
                            {
                                sendResponInstantMSG(CMFTServerMessage, oJsonObject.SeatID, oJsonObject.msg_id, station.Key, true, failedDeviceList.Contains(station.Key) ? station.Value : new List<string>());
                            }
                        }
                    }
                    /// CMFT TO DMD (4).    預錄訊息排程 
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting)oObject;
                        sLog += string.Format("排程ID = {0}；命令種類 = {1}；相關資料庫名稱1 = {2}；相關資料庫名稱2 = {3}", oJsonObject.sched_id, oJsonObject.command, oJsonObject.dbName1, oJsonObject.dbName2);
                    }
                    /// CMFT TO DMD (5).	預錄訊息設定
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting)oObject;
                        sLog += string.Format("預錄訊息ID = {0}；命令種類 = {1}；相關資料庫名稱1 = {2}", oJsonObject.msg_id, oJsonObject.command, oJsonObject.dbName1);
                    }
                    /// CMFT TO DMD (6).	列車訊息設定
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting)oObject;
                        sLog += string.Format("列車訊息ID = {0}；命令種類 = {1}；相關資料庫名稱1 = {2}", oJsonObject.msg_id, oJsonObject.command, oJsonObject.dbName1);
                    }
                    /// CMFT TO DMD (7).	電源設定
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting)oObject;
                        sLog += string.Format("命令種類 = {0}；相關資料庫名稱1 = {1}", oJsonObject.command, oJsonObject.dbName1);
                    }
                    /// CMFT TO DMD (8).	群組設定
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting)oObject;
                        sLog += string.Format("群組ID = {0}；命令種類 = {1}；相關資料庫名稱1 = {2}", oJsonObject.group_id, oJsonObject.command, oJsonObject.dbName1);
                    }
                    else if (sJsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting")
                    {
                        ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting)oObject;
                        sLog += string.Format("命令種類 = {0}；相關資料庫名稱1 = {1}", oJsonObject.command, oJsonObject.dbName1);
                    }
                    var temp = PrintJsonObject(CMFTServerMessage);
                    UpdateReceDataText(string.Format("{0} \r\n", temp));

                }

                else
                {
                    ///無此種訊息類別
                    sLog = string.Format("無此種訊息類別:[{0}]", CMFTServerMessage.MessageType);
                }

            }
            catch (System.Exception ex)
            {

                ASI.Lib.Log.ErrorLog.Log("TaskDMD", ex);
            }
        }
        #endregion  


        #region Thread
        private void UpdateReceDataText(string text)//解決跨執行緒的畫面問題    
        {
            if (ReceDataTextDCU.InvokeRequired)
            {
                // 檢查是否需要在 UI 線程上執行
                ReceDataTextDCU.Invoke((Action)(() => UpdateReceDataText(text)));
                // 使用 Invoke 方法來確保在 UI 線程上執行 
            }
            else
            {
                // 如果已經在 UI 線程上，則直接更新控制項
                ReceDataTextDCU.Text += text;
            }
        }
        private void UpdateBLackDataText(string text)
        {
            if (blackListText.InvokeRequired)
            {
                blackListText.Invoke((Action)(() => UpdateBLackDataText(text)));
            }
            else
            {
                blackListText.Text += "目前的狀態異常設備 : " + text + "\r\n";
            }
        }

        public void res_CMFTDate(string text)
        {
            if (resText.InvokeRequired)
            {
                resText.Invoke((Action)(() => res_CMFTDate(text)));
            }
            else
            {
                resText.Text += "回覆內容 : " + text + "\r\n";
            }
        }
        public void send_CMFTDate(string text)
        {
            if (sendDataText.InvokeRequired)
            {
                sendDataText.Invoke((Action)(() => send_CMFTDate(text)));
            }
            else
            {
                sendDataText.Text += "傳送內容 : " + text + "\r\n";
            }
        }
        private void UpdateWithSeparator(string separator)
        {
            if (blackListText.InvokeRequired)
            {
                blackListText.Invoke((Action)(() =>
                UpdateWithSeparator(separator)));
            }
            else
            {
                blackListText.Text += separator + Environment.NewLine;
            }
        }

        private void UpdateBlackListFromDatabase()
        {
            failedDeviceList.Clear();
            ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.SelectBlackList(false, "DMD", failedDeviceList);


            for (int i = 0; i < failedDeviceList.Count; i++)
            {
                var temp = failedDeviceList[i];

                UpdateBLackDataText(temp);
                // sendFuntion(failedDeviceList[i], false);
            }
            UpdateWithSeparator("================================");
        }
        private void UpdateConnetState(string connet)
        {
            if (connetText.InvokeRequired)
            {
                connetText.Invoke((Action)(() => UpdateConnetState(connet)));
            }
            else
            {
                connetText.Text += "連線狀態" + connet + "\r\n";
            }
        }
        #endregion
        #region     UI 

        private void button5_Click(object sender, EventArgs e)
        {
            InitializeConnection();
        }

        #endregion
        #region Method
        private void SendMessage(object source, ElapsedEventArgs e)
        {
            try
            {
                var MSG = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Heartbeat, 0, "連線中");
                CMFT_API.Send(MSG);
                string sRcvTime = System.DateTime.Now.ToString("HH:mm:ss.fff");

                UpdateConnetState("發送訊息與CMFT連線 HeartBeat" + sRcvTime);
                ASI.Lib.Log.DebugLog.Log("conetState", MSG.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void LoadDataGridViewData()
        {
            // 添加 equip_id 欄位 
            dataGridView1.Columns.Add("equip_id", "Equip ID");

            // 添加 equip_status 欄位
            dataGridView1.Columns.Add("equip_status", "Equip Status");

            var equip_Statuses = ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.SelectAll();

            try
            {
                // 使用 foreach 遍歷 equip_Statuses
                foreach (var status in equip_Statuses)
                {
                    // 創建一個 DataGridViewRow 並填充資料
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = status.equip_id });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = status.equip_status });

                    // 將 DataGridViewRow 添加到 DataGridView
                    dataGridView1.Rows.Add(row);

                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

        }

        private void LoadDateGridViewPanel()
        {

            dataGridViewPanel.Columns.Add("station_id", "車站");
            dataGridViewPanel.Columns.Add("area_id", "地區");
            dataGridViewPanel.Columns.Add("device_id", "裝置_id");
            dataGridViewPanel.Columns.Add("device_status", "設備狀態");
            var dmd_target = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectAll();
            try
            {
                // 使用 foreach 遍歷 equip_Statuses
                foreach (var target in dmd_target)
                {
                    // 創建一個 DataGridViewRow 並填充資料
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = target.station_id });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = target.area_id });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = target.device_id });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = target.device_status });
                    // 將 DataGridViewRow 添加到 DataGridView
                    dataGridViewPanel.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public void closeApi()
        {
            CMFT_API.ReceivedEvent -= CMFT_API_ReceivedEvent;
            CMFT_API.Dispose();
        }
        private void HandleError(Exception ex)
        {
            // 處理例外情況，例如日誌記錄或提示用戶
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        /// <summary>
        /// 送出設配資訊   
        /// </summary>
        public void sendFuntion(string equip_id, bool equip_status)
        {
            try
            {
                //建立設備狀態的json 物件 
                var equipStatus = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.EquipStatus(ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
                equipStatus.SeatID = "DMD";
                equipStatus.equip_id = equip_id;
                equipStatus.status = equip_status;
                equipStatus.dbName1 = "sys_equip_status";

                // 序列化 JSON 物件成為訊息內容  
                string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(equipStatus);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    // 建立要發送的訊息
                    ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
                    oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;
                    oMessage.MessageID = GenerateUniqueMessageID();
                    oMessage.JsonContent = jsonContent;
                    // 在這裡執行更動資料庫的步驟，例如更新資料庫中的設備狀態
                    UpdateEquipStatus(equip_id, equip_status);

                    // 發送訊息並處理結果
                    int sendResult = CMFT_API.Send(oMessage);
                    if (sendResult == 0)
                    {
                        // 發送成功，顯示成功訊息並記錄
                        // 印出時間、Json 內容
                        var temp = string.Empty;
                        temp += $"{DateTime.Now:HH:mm:ss.fff} {oMessage.JsonContent}\r\n";
                        temp += Print(oMessage.Content);
                        send_CMFTDate(temp);
                        ASI.Lib.Log.DebugLog.Log("SendToCMFTEquip", $"傳送封包內容 : {jsonContent}\r\n");
                    }
                    else
                    {
                        // 處理發送失敗的情況
                        HandleSendFailure(sendResult);
                    }
                }
            }
            catch (SyntaxErrorException EX)
            {
                MessageBox.Show("傳送出錯 原因:" + EX);
            }
        }
        /// <summary>
        /// 營運模式
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="Station"></param>
        public void sendOperation(string Mode, string Station)
        {
            //建立json物件
            var operationMode = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.OperationMode(ASI.Wanda.CMFT.Enum.COMDevice.BOCC_DMD_server);

            operationMode.SeatID = "DMD";
            operationMode.station_id = Station;
            operationMode.mode = Mode;
            // 序列化 JSON 物件成為訊息內容
            string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(operationMode);

            if (!string.IsNullOrEmpty(jsonContent))
            {
                // 建立要發送的訊息
                ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
                oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;
                oMessage.MessageID = GenerateUniqueMessageID();
                // 發送訊息並處理結果
                int sendResult = CMFT_API.Send(oMessage);
                if (sendResult == 0)
                {
                    // 發送成功，顯示成功訊息並記錄 
                    MessageBox.Show("成功發送訊息");
                    sendDataText.Text += $"{DateTime.Now:HH:mm:ss.fff} {oMessage.JsonContent}\r\n";
                }
                else
                {
                    // 處理發送失敗的情況
                    HandleSendFailure(sendResult);
                }

            }
        }
        #endregion

        #region private Method 
        private string Print(byte[] Data) // 將資料轉換成字串 
        {
            StringBuilder hexString = new StringBuilder(); // 創建一個新的 StringBuilder 來存儲結果
            hexString.Append("封包內容：");
            foreach (byte b in Data)
            {
                hexString.Append(b.ToString("X2")); // 將每個元素轉換為十六進位字串，並附加到 StringBuilder 
                hexString.Append(" "); // 在字元之間加入一個空格，以便更容易閱讀
            }
            hexString.AppendLine();
            return hexString.ToString(); // 返回 StringBuilder 中的結果 
        }
        private string PrintJsonObject(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
            string jsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(CMFTServerMessage.JsonContent, "JsonObjectName");
            var jsonObject = ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);

            var temp = string.Empty;
            Type t = jsonObject.GetType();
            temp += string.Format("MessageID:{0}", CMFTServerMessage.MessageID) + Environment.NewLine;
            temp += "==========================================================" + Environment.NewLine;
            temp += string.Format("時間: {0}", DateTime.Now.ToString()) + Environment.NewLine;
            temp += string.Format("類別: {0}", t.FullName) + Environment.NewLine;
            temp += "==========================================================" + Environment.NewLine;
            PropertyInfo[] props = t.GetProperties();
            temp += string.Format("屬性({0}個):", props.Length) + Environment.NewLine;

            foreach (var prop in props)
            {
                var Name = prop.Name;
                var TypeName = prop.PropertyType.Name;
                var Value = prop.GetValue(jsonObject);

                if (Value is IList)
                {
                    var ListValues = Value as IList;
                    temp += "-------------------------------------------------------------------------------------------" + Environment.NewLine;
                    temp += string.Format("　　屬性：{0} ({1}) 數量{2}:", Name, TypeName, ListValues.Count) + Environment.NewLine;
                    //temp += "------------------------------------" + Environment.NewLine; 

                    foreach (var listValue in ListValues)
                    {
                        temp += string.Format("　　　值({0})：{1}", listValue.GetType().Name, listValue) + Environment.NewLine;
                    }

                }
                else
                {
                    temp += "-------------------------------------------------------------------------------------------" + Environment.NewLine;
                    temp += string.Format("　　屬性：{0}:", Name) + Environment.NewLine;
                    //temp += "------------------------------------" + Environment.NewLine;
                    temp += string.Format("　　　值({0})：{1}", TypeName, Value) + Environment.NewLine;
                }
                //temp+= "------------------------------------" + Environment.NewLine;
            }
            temp += "-------------------------------------------------------------------------------------------" + Environment.NewLine;

            return temp;
        }
        private int GenerateUniqueMessageID()
        {
            // 實作一個方法來生成唯一的訊息識別碼
            return new Random().Next(1, 100000);
        }

        // 連線資料庫更新設備狀態
        private void UpdateEquipStatus(string equipId, bool newStatus)
        {

            ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.UpdateEquipStatus(equipId, newStatus);
        }
        private void HandleSendFailure(int sendResult)
        {
            // 根據 sendResult 的值，顯示相對應的錯誤訊息
            if (sendResult == -1)
            {
                MessageBox.Show("例外錯誤");
            }
            else if (sendResult == -2)
            {
                MessageBox.Show("-2：轉成byte[]封包時發生錯誤");
            }
            else if (sendResult == -3)
            {
                MessageBox.Show("未連線");
            }
            // 其他可能的錯誤處理...
        }
        private void SendResponsePreRecordMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string seatId, List<string> msgId, string stationId, bool isSuccess, List<string> failedTargets)
        {
            ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = CMFTServerMessage.MessageID;
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Response;

            var res_SendPreRecordMessage = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendPreRecordMessage(ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
            res_SendPreRecordMessage.SeatID = seatId;
            res_SendPreRecordMessage.msg_id = msgId;
            ///組成失敗的訊息內容
            res_SendPreRecordMessage.station_id = stationId;
            res_SendPreRecordMessage.is_success = isSuccess;
            res_SendPreRecordMessage.failed_target = isSuccess ? null : failedTargets;

            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(res_SendPreRecordMessage);
            CMFT_API.Send(oMessage);
            var temp = PrintJsonObject(oMessage);
            res_CMFTDate(temp);
            ASI.Lib.Log.DebugLog.Log("ResponPreRecord", oMessage.JsonContent);
        }
        // 創建並發送回應即時訊息的方法    
        private void sendResponInstantMSG(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string seatId, string msgId, string stationId, bool isSuccess, List<string> failedTargets)
        {
            ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = CMFTServerMessage.MessageID;
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Response;

            var res_SendInstantMessage = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendInstantMessage(ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
            res_SendInstantMessage.SeatID = seatId;
            res_SendInstantMessage.msg_id = msgId;
            //失敗的看板內容
            res_SendInstantMessage.station_id = stationId;
            res_SendInstantMessage.is_success = isSuccess;
            res_SendInstantMessage.failed_target = isSuccess ? null : failedTargets;

            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(res_SendInstantMessage);
            CMFT_API.Send(oMessage);
            var temp = PrintJsonObject(oMessage);
            res_CMFTDate(temp);
            ASI.Lib.Log.DebugLog.Log("ResponInstant", oMessage.JsonContent);
        }
        private void updateDMDPlayList()
        {

            List<ASI.Wanda.CMFT.DB.Models.DMD.dmd_playlist> tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPlayList.SelectAll();
            List<ASI.Wanda.DMD.DB.Models.dmd_playlist> convertedList = new List<ASI.Wanda.DMD.DB.Models.dmd_playlist>();

            foreach (var item in tempList)
            {
                ASI.Wanda.DMD.DB.Models.dmd_playlist convertedItem = new ASI.Wanda.DMD.DB.Models.dmd_playlist
                {
                    playlist_id = item.playlist_id,
                    station_id = item.station_id,
                    area_id = item.area_id,
                    device_id = item.device_id,
                    message_id = item.message_id,
                    message_type = item.message_type,
                    ins_time = item.ins_time,
                    ins_user = item.ins_user,
                    send_time = item.send_time,
                    upd_time = item.upd_time,
                    upd_user = item.upd_user,
                };
                convertedList.Add(convertedItem);
            }
            foreach (var item in convertedList)
            {
                //mSGtype  0 =預錄  1= 及時 
                ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.InsertPlayingItem(item.playlist_id, item.station_id, item.area_id, item.device_id, item.message_id, item.message_type, item.send_time);
            }
        }

        #endregion

        private void button6_Click(object sender, EventArgs e)
        {
            ASI.Wanda.CMFT.DB.Manager.Initializer("127.0.0.1", "5432", "CMFTDB", "postgres", "postgres", "admin");
            UpdateDMDPlayList();
            UpdataDMDPreRecordMessage();

        }
        /// <summary>
        /// 更新dmd_playlist的資料庫 
        /// </summary>  
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList> UpdateDMDPlayList()
        {
            try
            {
                ///抓取CMFT的資料表
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPlayList.SelectAll();
                ///轉換過程 
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_playlist
                    {
                        playlist_id = item.playlist_id,
                        station_id = item.station_id,
                        area_id = item.area_id,
                        device_id = item.device_id,
                        message_id = item.message_id,
                        message_type = item.message_type,
                        ins_time = item.ins_time,
                        ins_user = item.ins_user,
                        send_time = item.send_time,
                        upd_time = item.upd_time,
                        upd_user = item.upd_user,
                    })
                    .ToList();
                ///刪除原本的資料
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.DeletePlayingItem(
                        item.station_id, item.area_id, item.device_id);
                });

                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.InsertPlayingItem(
                        item.playlist_id,
                        item.station_id,
                        item.area_id,
                        item.device_id,
                        item.message_id,
                        item.message_type,
                        item.send_time
                    );
                }
                ///選擇並分類同一車站的數據 
                return convertedList.Cast<ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況
                ASI.Lib.Log.ErrorLog.Log("Error updating dmd_playlist", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList>();
            }

        }
        /// <summary>
        /// 更新DMDPreRecordMessage資料表  
        /// </summary>
        /// <returns></returns>    
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage> UpdataDMDPreRecordMessage()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPreRecordMessage.SelectAll();
                ///轉換過程  
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_pre_record_message
                    {
                        message_id = item.message_id,
                        message_name = item.message_name,
                        message_type = item.message_type,
                        message_priority = item.message_priority,
                        move_mode = item.move_mode,
                        move_speed = item.move_speed,
                        Interval = item.Interval,
                        message_content = item.message_content,
                        font_type = item.font_type,
                        font_size = item.font_size,
                        font_color = item.font_color,
                        message_content_en = item.message_content_en,
                        font_type_en = item.font_type_en,
                        font_size_en = item.font_size_en,
                        font_color_en = item.font_color_en,
                        ins_user = item.ins_user,
                        ins_time = item.ins_time,
                        upd_user = item.upd_user,
                        upd_time = item.upd_time,
                    })
                    .ToList();
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.DeletePreRecordMessage(
                       item.message_id
                    );
                });
                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.InsertPreRecordMessage(
                        item.message_id,
                        item.message_name,
                        item.message_type,
                        item.message_priority,
                        item.move_mode,
                        item.move_speed,
                        item.Interval,
                        item.message_content,
                        item.font_type,
                        item.font_size,
                        item.font_color,
                        item.message_content_en,
                        item.font_type_en,
                        item.font_size_en,
                        item.font_color_en
                    );
                }

                return convertedList.Cast<ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdPreRecordMessage", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage>();
            }
        }

    }
}
#endregion