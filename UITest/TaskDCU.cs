using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASI.Wanda.DMD;

namespace UITest
{
    public partial class TaskDCU : UserControl
    {

        #region Constructor
        ASI.Wanda.DMD.DMD_API mDMD_API = null;
        ASI.Wanda.DMD.DMD_API mDMD_API2 = null;

        public TaskDCU()
        {
            InitializeComponent();
            mDMD_API = new DMD_API();
            mDMD_API2 = new DMD_API();
            ReflashViewData();
        }

        #region DMD_ReceivedEvent
        private void DMD_API_ReceivedEvent(ASI.Wanda.DMD.Message.Message DCUServerMessage)
        {
            string sLog = "";
            try
            {
                string sRcvTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
                string sByteArray = ASI.Lib.Text.Parsing.String.BytesToHexString(DCUServerMessage.CompleteContent, "");
                string sJsonData = DCUServerMessage.JsonContent;
                string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "JsonObjectName");
                int iMsgID = DCUServerMessage.MessageID;

                if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Ack)
                {
                    ///Ack
                    sLog = string.Format("Ack，訊息識別碼:[{0}]", DCUServerMessage.MessageID);
                    MessageBox.Show("ACK");
                }
                else if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Command)
                {
                    sLog = $"從DCU Server收到:{sByteArray}；訊息類別碼:{DCUServerMessage.MessageType}；識別碼:{iMsgID}；長度:{DCUServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
                    ///Change/Command
                    sLog = string.Format("JsonObjectName = {0}\r\n", sJsonObjectName);
                    ///DCU Server to DMD
                    MessageBox.Show("command");

                }

            }
            catch (System.Exception ex)
            {

                ASI.Lib.Log.ErrorLog.Log("TaskDCU", ex);
            }
        }

        #endregion
        #region UI
        private void buttonInit_Click(object sender, EventArgs e)
        {
            var mProcName = "Socket";
            if (mDMD_API != null)
            {
                mDMD_API.Dispose();
                mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;

            }
            string sConn = string.Format($"IP={textBoxConnIP.Text};Port={textBoxConnPort.Text};Type={textBoxTYPE.Text}");
            mDMD_API.ReceivedEvent += DMD_API_ReceivedEvent;
            var iResult = mDMD_API.Initial(sConn);
            if (iResult == 0)
            {
                //  mIsConnectedToCMFT = true;
                MessageBox.Show("開啟成功");
                //連線成功時，重新計算最後一次收到DMD的時間  
                // LastHeartbeatTime = System.DateTime.Now;
            }
            else
            {
                //  mIsConnectedToCMFT = false;
                MessageBox.Show("開啟失敗");
            }

            //  MessageBox.Show("從新開起stocket");
        }
        private void buttonDispose_Click(object sender, EventArgs e)
        {
            if (mDMD_API != null)
            {
                mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                mDMD_API.Dispose();
                MessageBox.Show("斷開stocket");
            }
        }
        #endregion
        #region Method
        private void InitializeConnection()
        {
            try
            {
                string sConn = string.Format($"IP={textBoxConnIP.Text};Port={textBoxConnPort.Text};Type={textBoxTYPE.Text}");

                // 初始化 mDMD_API
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                    mDMD_API.Dispose();
                }
                mDMD_API.Initial(sConn);
                mDMD_API.ReceivedEvent += DMD_API_ReceivedEvent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化連接異常：" + ex.Message);
            }
        }
        // 初始化DataGridView的列标题和数据
        private void InitializeDataGridView(DataGridView dataGridView, string[] columnNames, List<object[]> data)
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            foreach (var columnName in columnNames)
            {
                dataGridView.Columns.Add(columnName, columnName);
            }
            foreach (var rowData in data)
            {
                DataGridViewRow row = new DataGridViewRow();
                foreach (var value in rowData)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = value });
                }
                dataGridView.Rows.Add(row);
            }
        }

        public void ReflashViewData()
        {
            // 添加要顯示的列表
            string[] dcuColumnNames = { "設備ID", "面板ID", "設備狀態" };
            InitializeDataGridView(DCUdataGridView, dcuColumnNames, GetAllData("DCU"));

            string[] dmdColumnNames = { "設備ID", "設備狀態" };
            InitializeDataGridView(DMDdataGridView, dmdColumnNames, GetAllData("DMD"));
        }

        private List<object[]> GetAllData(string type)
        {
            List<object[]> data = new List<object[]>();
            if (type == "DCU")
            {
                var du_list = ASI.Wanda.DCU.DB.Tables.DCU.dulist.SelectAll();
                foreach (var list in du_list)
                {
                    data.Add(new object[] { list.equip_id, list.panel_id, list.status });
                }
            }
            else if (type == "DMD")
            {
                var equip_status_list = ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.SelectAll();
                foreach (var equip_status in equip_status_list)
                {
                    data.Add(new object[] { equip_status.equip_id, equip_status.equip_status });
                }
            }
            return data;
        }


        private int GenerateUniqueMessageID()
        {
            // 實作一個方法來生成唯一的訊息識別碼
            return new Random().Next(1, 100000);
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


        private void DMDdataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = DCUdataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // 嘗試將 newValue 轉換為 bool

                if (int.TryParse(cell.Value.ToString(), out int equip_status))
                {
                    // 獲取對應的 equip_id
                    string equipId = DCUdataGridView.Rows[e.RowIndex].Cells["equip_id"].Value.ToString();
                    string panelId = DCUdataGridView.Rows[e.RowIndex].Cells["panel_id"].Value.ToString();

                }
                else
                {
                    // 如果無法轉換為 bool，你可以處理錯誤或顯示錯誤訊息
                    MessageBox.Show("無法解析為布林值！");
                }
            }

        }


        private void UpdateEquipStatus(string equipId, string panelID, int newStatus)
        {

            ASI.Wanda.DCU.DB.Tables.DCU.dulist.UpdateEquipStatus(equipId, panelID, newStatus);
        }
        public void closeApi()
        {

            mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
            mDMD_API.Dispose();
        }



        private bool ConvertToBool(int equip_status)
        {
            return equip_status == 0;
        }
        #endregion

        private void testBtn_Click(object sender, EventArgs e)
        {
            ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = GenerateUniqueMessageID();
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;

            var SendPreRecordMessage = new ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage("001");
            SendPreRecordMessage.SeatID = "TEST";
            SendPreRecordMessage.msg_id.Add("測試內容");
            SendPreRecordMessage.target_du = new List<string>
            {
                "LG01_CCS_CDU-1",
                "LG01_CCS_CDU-2",
                "LG01_UPF_PDU-1",
                "LG08A_DPF_PDU-4"
            };
            SendPreRecordMessage.dbName1 = "dmd_pre_record_message";
            SendPreRecordMessage.dbName2 = "dmd_target";

            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(SendPreRecordMessage);

            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(oMessage.JsonContent);
            //組封包
            var sendPreRecordMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
            sendPreRecordMessage.msg_id = oJsonObject.msg_id;
            sendPreRecordMessage.target_du = oJsonObject.target_du;
            sendPreRecordMessage.seatID = oJsonObject.SeatID;

            var ObjectName = ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage);
            //組成給DCU的封包
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, oMessage.MessageID, ObjectName);
            var RESLUT = mDMD_API.Send(MSG);
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCU", MSG.JsonContent);
            MessageBox.Show("傳送:" + RESLUT.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var mProcName = "Socket";
            if (mDMD_API2 != null)
            {
                mDMD_API2.Dispose();
                mDMD_API2.ReceivedEvent -= DMD_API_ReceivedEvent;

            }
            string sConn = string.Format($"IP={textBox3.Text};Port={textBox2.Text};Type={textBox1.Text}");
            mDMD_API2.ReceivedEvent += DMD_API_ReceivedEvent;
            var iResult = mDMD_API2.Initial(sConn);

            bool inUse = IsPortInUse(int.Parse(textBox2.Text));

            if (inUse)
            {
                MessageBox.Show($"Port {textBox2} is in use.");
            }
            else
            {
                MessageBox.Show($"Port {textBox2} is available.");
            }


            if (iResult == 0)
            {
                //  mIsConnectedToCMFT = true;
                MessageBox.Show("開啟成功");
                //連線成功時，重新計算最後一次收到DMD的時間  
                // LastHeartbeatTime = System.DateTime.Now;
            }
            else
            {
                //  mIsConnectedToCMFT = false;
                MessageBox.Show("開啟失敗");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mDMD_API2 != null)
            {
                mDMD_API2.ReceivedEvent -= DMD_API_ReceivedEvent;
                mDMD_API2.Dispose();

                MessageBox.Show("斷開stocket");
            }
        }


        public static bool IsPortInUse(int port)
        {
            bool isPortInUse = false;
            TcpListener tcpListener = null;

            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
            }
            catch (SocketException)
            {
                isPortInUse = true;
            }
            finally
            {
                tcpListener?.Stop();
            }

            return isPortInUse;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ASI.Wanda.CMFT.Message.Message oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = GenerateUniqueMessageID();
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;

            var PowerTimeSetting = new ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting("001");
            PowerTimeSetting.SeatID = "TEST";
            //SendPreRecordMessage.msg_id.Add("測試內容");
            //SendPreRecordMessage.target_du = new List<string>
            //{
            //    "LG01_CCS_CDU-1",
            //    "LG01_CCS_CDU-2",
            //    "LG01_UPF_PDU-1",
            //    "LG08A_DPF_PDU-4"
            //};
            PowerTimeSetting.command = ASI.Wanda.CMFT.Enum.SqlCommand.update;
            PowerTimeSetting.dbName1 = "dmd_power_setting";


            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(PowerTimeSetting);

            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(oMessage.JsonContent);
            //組封包
            var PowerTimeSettingDMD = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting(ASI.Wanda.DMD.Enum.Station.LG06);

            PowerTimeSettingDMD.seatID = oJsonObject.SeatID;
            PowerTimeSettingDMD.SqlCommand = ASI.Wanda.DMD.Enum.SqlCommand.update;

            var ObjectName = ASI.Lib.Text.Parsing.Json.SerializeObject(PowerTimeSettingDMD);
            //組成給DCU的封包
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, oMessage.MessageID, ObjectName);
            var RESLUT = mDMD_API.Send(MSG);
            ASI.Lib.Log.DebugLog.Log("SendPowerTimeSettingToDCU", MSG.JsonContent);
            MessageBox.Show("傳送:" + RESLUT.ToString());
        }
    }
}
#endregion