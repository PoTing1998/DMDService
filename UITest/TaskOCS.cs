using DMDService.Services.Interfaces;
using System;
using System.Windows.Forms;

namespace UITest
{
    public partial class TaskOCS : UserControl
    {
        #region Fields

        private readonly IOcsService _ocsService;

        #endregion

        #region Constructors

        /// <summary>
        /// 無參數建構子，供 Designer 使用
        /// </summary>
        public TaskOCS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 注入建構子，供執行期使用
        /// </summary>
        public TaskOCS(IOcsService ocsService) : this()
        {
            _ocsService = ocsService ?? throw new ArgumentNullException(nameof(ocsService));

            // 訂閱 Service 事件
            _ocsService.LogData += OnLogData;
            _ocsService.LogError += text => MessageBox.Show(text, "OCS 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Event Handlers

        private void buttonInit_Click_1(object sender, EventArgs e)
        {
            if (_ocsService == null) return;

            try
            {
                byte slaveAddress = BitConverter.GetBytes(int.Parse(slaveAddressText.Text))[0];
                ushort startAddress = ushort.Parse(adressText.Text);
                ushort endAddress = ushort.Parse(adressTextEND.Text);
                int port = int.Parse(textBoxConnPort.Text);

                _ocsService.ConnectAndRead(textBoxConnIP.Text, port, slaveAddress, startAddress, endAddress);
                _ocsService.StartOCSPolling();

                SetStatus(polling: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("參數錯誤: " + ex.Message, "OCS 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void stopButton_Click_1(object sender, EventArgs e)
        {
            _ocsService?.Stop();
            SetStatus(polling: false);
        }

        private void clearBT_Click_1(object sender, EventArgs e)
        {
            ReceDataText.Text = string.Empty;
            ReceDataText2.Text = string.Empty;
            ReceDataText3.Text = string.Empty;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 依 channel 將 Service 回傳的文字附加到對應 RichTextBox
        /// channel 1 = ReceDataText（地址 30001-30600）
        /// channel 2 = ReceDataText2（地址 30601-31200）
        /// channel 3 = ReceDataText3（地址 31201-31800）
        /// </summary>
        private void OnLogData(int channel, string text)
        {
            RichTextBox target;
            switch (channel)
            {
                case 2:  target = ReceDataText2; break;
                case 3:  target = ReceDataText3; break;
                default: target = ReceDataText;  break;
            }
            AppendTextSafe(target, text);
        }

        private void AppendTextSafe(RichTextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
                textBox.Invoke((Action)(() => AppendTextSafe(textBox, text)));
            else
                textBox.AppendText(text);
        }

        /// <summary>
        /// 視窗尺寸改變時自動等分三欄並撐滿高度
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            DistributeLayout();
        }

        private void DistributeLayout()
        {
            if (grpSettings == null || grpClient1 == null) return;

            const int margin  = 3;
            const int gap     = 3;
            const int settingsTop    = 3;
            const int settingsHeight = 110;
            const int clientsTop     = settingsTop + settingsHeight + gap;

            int totalW = ClientSize.Width;
            int totalH = ClientSize.Height;

            // 頂部設定欄：橫向拉滿
            grpSettings.SetBounds(margin, settingsTop,
                                  totalW - margin * 2, settingsHeight);

            // 三欄等寬，餘數補給最後一欄
            int available = totalW - margin * 2 - gap * 2;
            int colW      = available / 3;
            int col3W     = available - colW * 2;   // 吃掉整除餘數
            int colH      = totalH - clientsTop - margin;

            grpClient1.SetBounds(margin,                    clientsTop, colW,  colH);
            grpClient2.SetBounds(margin + colW + gap,       clientsTop, colW,  colH);
            grpClient3.SetBounds(margin + colW * 2 + gap * 2, clientsTop, col3W, colH);
        }

        /// <summary>
        /// 更新連線狀態 Label（thread-safe）
        /// </summary>
        private void SetStatus(bool polling)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke((Action)(() => SetStatus(polling)));
                return;
            }

            if (polling)
            {
                lblStatus.Text      = "● 輪詢中";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblStatus.Text      = "● 未連線";
                lblStatus.ForeColor = System.Drawing.Color.Gray;
            }
        }

        #endregion
    }
}
