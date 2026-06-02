namespace UITest
{
    partial class TaskOCS
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        private void InitializeComponent()
        {
            System.Drawing.Font defaultFont = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

            // ── 頂部：連線設定 GroupBox ──────────────────────────────────────
            this.grpSettings    = new System.Windows.Forms.GroupBox();
            this.label3         = new System.Windows.Forms.Label();
            this.textBoxConnIP  = new System.Windows.Forms.TextBox();
            this.label4         = new System.Windows.Forms.Label();
            this.textBoxConnPort = new System.Windows.Forms.TextBox();
            this.label1         = new System.Windows.Forms.Label();
            this.slaveAddressText = new System.Windows.Forms.TextBox();
            this.label5         = new System.Windows.Forms.Label();
            this.adressText     = new System.Windows.Forms.TextBox();
            this.label6         = new System.Windows.Forms.Label();
            this.adressTextEND  = new System.Windows.Forms.TextBox();
            this.buttonInit     = new System.Windows.Forms.Button();
            this.stopButton     = new System.Windows.Forms.Button();
            this.clearBT        = new System.Windows.Forms.Button();
            this.lblStatus      = new System.Windows.Forms.Label();

            // ── 下方三欄 GroupBox ─────────────────────────────────────────────
            this.grpClient1     = new System.Windows.Forms.GroupBox();
            this.ReceDataText   = new System.Windows.Forms.RichTextBox();
            this.grpClient2     = new System.Windows.Forms.GroupBox();
            this.ReceDataText2  = new System.Windows.Forms.RichTextBox();
            this.grpClient3     = new System.Windows.Forms.GroupBox();
            this.ReceDataText3  = new System.Windows.Forms.RichTextBox();

            this.grpSettings.SuspendLayout();
            this.grpClient1.SuspendLayout();
            this.grpClient2.SuspendLayout();
            this.grpClient3.SuspendLayout();
            this.SuspendLayout();

            // ──────────────────────────────────────────────────────────────────
            // grpSettings
            // ──────────────────────────────────────────────────────────────────
            this.grpSettings.Font     = defaultFont;
            this.grpSettings.Location = new System.Drawing.Point(3, 3);
            this.grpSettings.Name     = "grpSettings";
            this.grpSettings.Size     = new System.Drawing.Size(837, 110);
            this.grpSettings.TabIndex = 0;
            this.grpSettings.TabStop  = false;
            this.grpSettings.Text     = "連線設定";
            this.grpSettings.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                      | System.Windows.Forms.AnchorStyles.Left;
            this.grpSettings.Controls.Add(this.label3);
            this.grpSettings.Controls.Add(this.textBoxConnIP);
            this.grpSettings.Controls.Add(this.label4);
            this.grpSettings.Controls.Add(this.textBoxConnPort);
            this.grpSettings.Controls.Add(this.label1);
            this.grpSettings.Controls.Add(this.slaveAddressText);
            this.grpSettings.Controls.Add(this.label5);
            this.grpSettings.Controls.Add(this.adressText);
            this.grpSettings.Controls.Add(this.label6);
            this.grpSettings.Controls.Add(this.adressTextEND);
            this.grpSettings.Controls.Add(this.buttonInit);
            this.grpSettings.Controls.Add(this.stopButton);
            this.grpSettings.Controls.Add(this.clearBT);
            this.grpSettings.Controls.Add(this.lblStatus);

            // ── Label 與 TextBox 第一排 (y=18 / y=38) ────────────────────────
            // ATS Server IP
            this.label3.AutoSize  = true;
            this.label3.Font      = defaultFont;
            this.label3.Location  = new System.Drawing.Point(8, 20);
            this.label3.Name      = "label3";
            this.label3.Text      = "ATS Server IP";

            this.textBoxConnIP.Font     = defaultFont;
            this.textBoxConnIP.Location = new System.Drawing.Point(8, 38);
            this.textBoxConnIP.Name     = "textBoxConnIP";
            this.textBoxConnIP.Size     = new System.Drawing.Size(95, 22);
            this.textBoxConnIP.TabIndex = 0;
            this.textBoxConnIP.Text     = "10.107.26.99";

            // Port
            this.label4.AutoSize  = true;
            this.label4.Font      = defaultFont;
            this.label4.Location  = new System.Drawing.Point(112, 20);
            this.label4.Name      = "label4";
            this.label4.Text      = "Port";

            this.textBoxConnPort.Font     = defaultFont;
            this.textBoxConnPort.Location = new System.Drawing.Point(112, 38);
            this.textBoxConnPort.Name     = "textBoxConnPort";
            this.textBoxConnPort.Size     = new System.Drawing.Size(55, 22);
            this.textBoxConnPort.TabIndex = 1;
            this.textBoxConnPort.Text     = "502";

            // Device ID
            this.label1.AutoSize  = true;
            this.label1.Font      = defaultFont;
            this.label1.Location  = new System.Drawing.Point(176, 20);
            this.label1.Name      = "label1";
            this.label1.Text      = "Device ID";

            this.slaveAddressText.Font     = defaultFont;
            this.slaveAddressText.Location = new System.Drawing.Point(176, 38);
            this.slaveAddressText.Name     = "slaveAddressText";
            this.slaveAddressText.Size     = new System.Drawing.Size(55, 22);
            this.slaveAddressText.TabIndex = 2;
            this.slaveAddressText.Text     = "0";

            // 起始位址
            this.label5.AutoSize  = true;
            this.label5.Font      = defaultFont;
            this.label5.Location  = new System.Drawing.Point(240, 20);
            this.label5.Name      = "label5";
            this.label5.Text      = "起始位址";

            this.adressText.Font     = defaultFont;
            this.adressText.Location = new System.Drawing.Point(240, 38);
            this.adressText.Name     = "adressText";
            this.adressText.Size     = new System.Drawing.Size(65, 22);
            this.adressText.TabIndex = 3;
            this.adressText.Text     = "30001";

            // 結束位址
            this.label6.AutoSize  = true;
            this.label6.Font      = defaultFont;
            this.label6.Location  = new System.Drawing.Point(314, 20);
            this.label6.Name      = "label6";
            this.label6.Text      = "結束位址";

            this.adressTextEND.Font     = defaultFont;
            this.adressTextEND.Location = new System.Drawing.Point(314, 38);
            this.adressTextEND.Name     = "adressTextEND";
            this.adressTextEND.Size     = new System.Drawing.Size(65, 22);
            this.adressTextEND.TabIndex = 4;
            this.adressTextEND.Text     = "30539";

            // ── 按鈕第二排 (y=72) ─────────────────────────────────────────────
            this.buttonInit.Font     = defaultFont;
            this.buttonInit.Location = new System.Drawing.Point(8, 72);
            this.buttonInit.Name     = "buttonInit";
            this.buttonInit.Size     = new System.Drawing.Size(90, 26);
            this.buttonInit.TabIndex = 5;
            this.buttonInit.Text     = "連線並讀取";
            this.buttonInit.UseVisualStyleBackColor = true;
            this.buttonInit.Click   += new System.EventHandler(this.buttonInit_Click_1);

            this.stopButton.Font     = defaultFont;
            this.stopButton.Location = new System.Drawing.Point(106, 72);
            this.stopButton.Name     = "stopButton";
            this.stopButton.Size     = new System.Drawing.Size(75, 26);
            this.stopButton.TabIndex = 6;
            this.stopButton.Text     = "停止輪詢";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click   += new System.EventHandler(this.stopButton_Click_1);

            this.clearBT.Font     = defaultFont;
            this.clearBT.Location = new System.Drawing.Point(189, 72);
            this.clearBT.Name     = "clearBT";
            this.clearBT.Size     = new System.Drawing.Size(75, 26);
            this.clearBT.TabIndex = 7;
            this.clearBT.Text     = "清除畫面";
            this.clearBT.UseVisualStyleBackColor = true;
            this.clearBT.Click   += new System.EventHandler(this.clearBT_Click_1);

            // ── 連線狀態指示 ───────────────────────────────────────────────────
            this.lblStatus.AutoSize  = true;
            this.lblStatus.Font      = new System.Drawing.Font("Microsoft JhengHei", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location  = new System.Drawing.Point(276, 77);
            this.lblStatus.Name      = "lblStatus";
            this.lblStatus.Text      = "● 未連線";

            // ──────────────────────────────────────────────────────────────────
            // grpClient1 — Client 1（30001～30600）
            // ──────────────────────────────────────────────────────────────────
            this.grpClient1.Font     = defaultFont;
            this.grpClient1.Location = new System.Drawing.Point(3, 120);
            this.grpClient1.Name     = "grpClient1";
            this.grpClient1.Size     = new System.Drawing.Size(278, 514);
            this.grpClient1.TabIndex = 1;
            this.grpClient1.TabStop  = false;
            this.grpClient1.Text     = "Client 1（30001～30600）";
            this.grpClient1.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                     | System.Windows.Forms.AnchorStyles.Left;
            this.grpClient1.Controls.Add(this.ReceDataText);

            this.ReceDataText.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);
            this.ReceDataText.Font      = new System.Drawing.Font("Consolas", 8.25F);
            this.ReceDataText.Location  = new System.Drawing.Point(3, 20);
            this.ReceDataText.Name      = "ReceDataText";
            this.ReceDataText.Size      = new System.Drawing.Size(272, 488);
            this.ReceDataText.TabIndex  = 0;
            this.ReceDataText.Text      = "";
            this.ReceDataText.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                        | System.Windows.Forms.AnchorStyles.Left
                                        | System.Windows.Forms.AnchorStyles.Bottom
                                        | System.Windows.Forms.AnchorStyles.Right;

            // ──────────────────────────────────────────────────────────────────
            // grpClient2 — Client 2（30601～31200）
            // ──────────────────────────────────────────────────────────────────
            this.grpClient2.Font     = defaultFont;
            this.grpClient2.Location = new System.Drawing.Point(284, 120);
            this.grpClient2.Name     = "grpClient2";
            this.grpClient2.Size     = new System.Drawing.Size(278, 514);
            this.grpClient2.TabIndex = 2;
            this.grpClient2.TabStop  = false;
            this.grpClient2.Text     = "Client 2（30601～31200）";
            this.grpClient2.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                     | System.Windows.Forms.AnchorStyles.Left;
            this.grpClient2.Controls.Add(this.ReceDataText2);

            this.ReceDataText2.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);
            this.ReceDataText2.Font      = new System.Drawing.Font("Consolas", 8.25F);
            this.ReceDataText2.Location  = new System.Drawing.Point(3, 20);
            this.ReceDataText2.Name      = "ReceDataText2";
            this.ReceDataText2.Size      = new System.Drawing.Size(272, 488);
            this.ReceDataText2.TabIndex  = 0;
            this.ReceDataText2.Text      = "";
            this.ReceDataText2.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                         | System.Windows.Forms.AnchorStyles.Left
                                         | System.Windows.Forms.AnchorStyles.Bottom
                                         | System.Windows.Forms.AnchorStyles.Right;

            // ──────────────────────────────────────────────────────────────────
            // grpClient3 — Client 3（31201～31800）
            // ──────────────────────────────────────────────────────────────────
            this.grpClient3.Font     = defaultFont;
            this.grpClient3.Location = new System.Drawing.Point(565, 120);
            this.grpClient3.Name     = "grpClient3";
            this.grpClient3.Size     = new System.Drawing.Size(278, 514);
            this.grpClient3.TabIndex = 3;
            this.grpClient3.TabStop  = false;
            this.grpClient3.Text     = "Client 3（31201～31800）";
            this.grpClient3.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                     | System.Windows.Forms.AnchorStyles.Left;
            this.grpClient3.Controls.Add(this.ReceDataText3);

            this.ReceDataText3.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);
            this.ReceDataText3.Font      = new System.Drawing.Font("Consolas", 8.25F);
            this.ReceDataText3.Location  = new System.Drawing.Point(3, 20);
            this.ReceDataText3.Name      = "ReceDataText3";
            this.ReceDataText3.Size      = new System.Drawing.Size(272, 488);
            this.ReceDataText3.TabIndex  = 0;
            this.ReceDataText3.Text      = "";
            this.ReceDataText3.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                         | System.Windows.Forms.AnchorStyles.Left
                                         | System.Windows.Forms.AnchorStyles.Bottom
                                         | System.Windows.Forms.AnchorStyles.Right;

            // ──────────────────────────────────────────────────────────────────
            // TaskOCS UserControl
            // ──────────────────────────────────────────────────────────────────
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font                = defaultFont;
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.grpClient1);
            this.Controls.Add(this.grpClient2);
            this.Controls.Add(this.grpClient3);
            this.Name = "TaskOCS";
            this.Size = new System.Drawing.Size(846, 637);

            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpClient1.ResumeLayout(false);
            this.grpClient2.ResumeLayout(false);
            this.grpClient3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // ── 連線設定 ──────────────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label    label3;
        private System.Windows.Forms.TextBox  textBoxConnIP;
        private System.Windows.Forms.Label    label4;
        private System.Windows.Forms.TextBox  textBoxConnPort;
        private System.Windows.Forms.Label    label1;
        private System.Windows.Forms.TextBox  slaveAddressText;
        private System.Windows.Forms.Label    label5;
        private System.Windows.Forms.TextBox  adressText;
        private System.Windows.Forms.Label    label6;
        private System.Windows.Forms.TextBox  adressTextEND;
        private System.Windows.Forms.Button   buttonInit;
        private System.Windows.Forms.Button   stopButton;
        private System.Windows.Forms.Button   clearBT;
        private System.Windows.Forms.Label    lblStatus;

        // ── 資料顯示三欄 ──────────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox   grpClient1;
        private System.Windows.Forms.RichTextBox ReceDataText;
        private System.Windows.Forms.GroupBox   grpClient2;
        private System.Windows.Forms.RichTextBox ReceDataText2;
        private System.Windows.Forms.GroupBox   grpClient3;
        private System.Windows.Forms.RichTextBox ReceDataText3;
    }
}
