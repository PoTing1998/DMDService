namespace UITest
{
    partial class SendToDCU
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxDatabase = new System.Windows.Forms.GroupBox();
            this.btnConnectDB = new System.Windows.Forms.Button();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.txtDBUserID = new System.Windows.Forms.TextBox();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.txtDBPort = new System.Windows.Forms.TextBox();
            this.txtDBIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxInstantMessage = new System.Windows.Forms.GroupBox();
            this.btnSaveInstantMessage = new System.Windows.Forms.Button();
            this.btnLoadInstantMessage = new System.Windows.Forms.Button();
            this.txtInstantMessageENG = new System.Windows.Forms.TextBox();
            this.labelInstantMessageENG = new System.Windows.Forms.Label();
            this.txtInstantMessageCHN = new System.Windows.Forms.TextBox();
            this.labelInstantMessageCHN = new System.Windows.Forms.Label();
            this.groupBoxMessage = new System.Windows.Forms.GroupBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnRefreshTargetDU = new System.Windows.Forms.Button();
            this.clbTargetDU = new System.Windows.Forms.ListView();
            this.label7 = new System.Windows.Forms.Label();
            this.btnRefreshMessageID = new System.Windows.Forms.Button();
            this.cboMessageIDEn = new System.Windows.Forms.ComboBox();
            this.label6En = new System.Windows.Forms.Label();
            this.cboMessageID = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSeatID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboStation = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.labelArea = new System.Windows.Forms.Label();
            this.cboMessageType = new System.Windows.Forms.ComboBox();
            this.labelMessageType = new System.Windows.Forms.Label();
            this.cboPriority = new System.Windows.Forms.ComboBox();
            this.labelPriority = new System.Windows.Forms.Label();
            this.cboMoveSpeed = new System.Windows.Forms.ComboBox();
            this.labelMoveSpeed = new System.Windows.Forms.Label();
            this.cboMoveMode = new System.Windows.Forms.ComboBox();
            this.labelMoveMode = new System.Windows.Forms.Label();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.btnExportLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxDatabase.SuspendLayout();
            this.groupBoxInstantMessage.SuspendLayout();
            this.groupBoxMessage.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.btnDisconnect);
            this.groupBoxConnection.Controls.Add(this.btnConnect);
            this.groupBoxConnection.Controls.Add(this.cboType);
            this.groupBoxConnection.Controls.Add(this.txtPort);
            this.groupBoxConnection.Controls.Add(this.txtIP);
            this.groupBoxConnection.Controls.Add(this.label3);
            this.groupBoxConnection.Controls.Add(this.label2);
            this.groupBoxConnection.Controls.Add(this.label1);
            this.groupBoxConnection.Location = new System.Drawing.Point(15, 15);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(730, 100);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "連線設定";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(630, 55);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(80, 30);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "斷線";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(630, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 30);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "連線";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(470, 30);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(140, 20);
            this.cboType.TabIndex = 5;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(290, 30);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 22);
            this.txtPort.TabIndex = 4;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(80, 30);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(150, 22);
            this.txtIP.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "連線類型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP 位址:";
            // 
            // groupBoxDatabase
            // 
            this.groupBoxDatabase.Controls.Add(this.btnConnectDB);
            this.groupBoxDatabase.Controls.Add(this.txtDBPassword);
            this.groupBoxDatabase.Controls.Add(this.txtDBUserID);
            this.groupBoxDatabase.Controls.Add(this.txtDBName);
            this.groupBoxDatabase.Controls.Add(this.txtDBPort);
            this.groupBoxDatabase.Controls.Add(this.txtDBIP);
            this.groupBoxDatabase.Controls.Add(this.label8);
            this.groupBoxDatabase.Controls.Add(this.label9);
            this.groupBoxDatabase.Controls.Add(this.label10);
            this.groupBoxDatabase.Controls.Add(this.label11);
            this.groupBoxDatabase.Controls.Add(this.label12);
            this.groupBoxDatabase.Location = new System.Drawing.Point(15, 125);
            this.groupBoxDatabase.Name = "groupBoxDatabase";
            this.groupBoxDatabase.Size = new System.Drawing.Size(730, 100);
            this.groupBoxDatabase.TabIndex = 1;
            this.groupBoxDatabase.TabStop = false;
            this.groupBoxDatabase.Text = "資料庫連線設定";
            // 
            // btnConnectDB
            // 
            this.btnConnectDB.Location = new System.Drawing.Point(630, 55);
            this.btnConnectDB.Name = "btnConnectDB";
            this.btnConnectDB.Size = new System.Drawing.Size(80, 30);
            this.btnConnectDB.TabIndex = 10;
            this.btnConnectDB.Text = "連線資料庫";
            this.btnConnectDB.UseVisualStyleBackColor = true;
            this.btnConnectDB.Click += new System.EventHandler(this.btnConnectDB_Click);
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Location = new System.Drawing.Point(470, 65);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.PasswordChar = '*';
            this.txtDBPassword.Size = new System.Drawing.Size(140, 22);
            this.txtDBPassword.TabIndex = 9;
            this.txtDBPassword.Text = "postgres";
            // 
            // txtDBUserID
            // 
            this.txtDBUserID.Location = new System.Drawing.Point(470, 30);
            this.txtDBUserID.Name = "txtDBUserID";
            this.txtDBUserID.Size = new System.Drawing.Size(140, 22);
            this.txtDBUserID.TabIndex = 8;
            this.txtDBUserID.Text = "postgres";
            // 
            // txtDBName
            // 
            this.txtDBName.Location = new System.Drawing.Point(290, 65);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(100, 22);
            this.txtDBName.TabIndex = 7;
            this.txtDBName.Text = "DMDDB";
            // 
            // txtDBPort
            // 
            this.txtDBPort.Location = new System.Drawing.Point(290, 30);
            this.txtDBPort.Name = "txtDBPort";
            this.txtDBPort.Size = new System.Drawing.Size(100, 22);
            this.txtDBPort.TabIndex = 6;
            this.txtDBPort.Text = "5432";
            // 
            // txtDBIP
            // 
            this.txtDBIP.Location = new System.Drawing.Point(80, 30);
            this.txtDBIP.Name = "txtDBIP";
            this.txtDBIP.Size = new System.Drawing.Size(150, 22);
            this.txtDBIP.TabIndex = 5;
            this.txtDBIP.Text = "127.0.0.1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(410, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "Password:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(410, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "User ID:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(236, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "DB名稱:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(250, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "Port:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 33);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "IP 位址:";
            // 
            // groupBoxInstantMessage
            // 
            this.groupBoxInstantMessage.Controls.Add(this.btnSaveInstantMessage);
            this.groupBoxInstantMessage.Controls.Add(this.btnLoadInstantMessage);
            this.groupBoxInstantMessage.Controls.Add(this.txtInstantMessageENG);
            this.groupBoxInstantMessage.Controls.Add(this.labelInstantMessageENG);
            this.groupBoxInstantMessage.Controls.Add(this.txtInstantMessageCHN);
            this.groupBoxInstantMessage.Controls.Add(this.labelInstantMessageCHN);
            this.groupBoxInstantMessage.Location = new System.Drawing.Point(15, 630);
            this.groupBoxInstantMessage.Name = "groupBoxInstantMessage";
            this.groupBoxInstantMessage.Size = new System.Drawing.Size(730, 135);
            this.groupBoxInstantMessage.TabIndex = 4;
            this.groupBoxInstantMessage.TabStop = false;
            this.groupBoxInstantMessage.Text = "即時訊息編輯";
            // 
            // btnSaveInstantMessage
            // 
            this.btnSaveInstantMessage.Location = new System.Drawing.Point(260, 100);
            this.btnSaveInstantMessage.Name = "btnSaveInstantMessage";
            this.btnSaveInstantMessage.Size = new System.Drawing.Size(120, 25);
            this.btnSaveInstantMessage.TabIndex = 5;
            this.btnSaveInstantMessage.Text = "保存訊息";
            this.btnSaveInstantMessage.UseVisualStyleBackColor = true;
            this.btnSaveInstantMessage.Click += new System.EventHandler(this.btnSaveInstantMessage_Click);
            // 
            // btnLoadInstantMessage
            // 
            this.btnLoadInstantMessage.Location = new System.Drawing.Point(120, 100);
            this.btnLoadInstantMessage.Name = "btnLoadInstantMessage";
            this.btnLoadInstantMessage.Size = new System.Drawing.Size(120, 25);
            this.btnLoadInstantMessage.TabIndex = 4;
            this.btnLoadInstantMessage.Text = "載入選定訊息";
            this.btnLoadInstantMessage.UseVisualStyleBackColor = true;
            this.btnLoadInstantMessage.Click += new System.EventHandler(this.btnLoadInstantMessage_Click);
            // 
            // txtInstantMessageENG
            // 
            this.txtInstantMessageENG.Location = new System.Drawing.Point(120, 65);
            this.txtInstantMessageENG.Multiline = true;
            this.txtInstantMessageENG.Name = "txtInstantMessageENG";
            this.txtInstantMessageENG.Size = new System.Drawing.Size(500, 25);
            this.txtInstantMessageENG.TabIndex = 3;
            // 
            // labelInstantMessageENG
            // 
            this.labelInstantMessageENG.AutoSize = true;
            this.labelInstantMessageENG.Location = new System.Drawing.Point(20, 68);
            this.labelInstantMessageENG.Name = "labelInstantMessageENG";
            this.labelInstantMessageENG.Size = new System.Drawing.Size(80, 12);
            this.labelInstantMessageENG.TabIndex = 2;
            this.labelInstantMessageENG.Text = "英文訊息內容:";
            // 
            // txtInstantMessageCHN
            // 
            this.txtInstantMessageCHN.Location = new System.Drawing.Point(120, 30);
            this.txtInstantMessageCHN.Multiline = true;
            this.txtInstantMessageCHN.Name = "txtInstantMessageCHN";
            this.txtInstantMessageCHN.Size = new System.Drawing.Size(500, 25);
            this.txtInstantMessageCHN.TabIndex = 1;
            // 
            // labelInstantMessageCHN
            // 
            this.labelInstantMessageCHN.AutoSize = true;
            this.labelInstantMessageCHN.Location = new System.Drawing.Point(20, 33);
            this.labelInstantMessageCHN.Name = "labelInstantMessageCHN";
            this.labelInstantMessageCHN.Size = new System.Drawing.Size(80, 12);
            this.labelInstantMessageCHN.TabIndex = 0;
            this.labelInstantMessageCHN.Text = "中文訊息內容:";
            // 
            // groupBoxMessage
            // 
            this.groupBoxMessage.Controls.Add(this.btnSend);
            this.groupBoxMessage.Controls.Add(this.btnRefreshTargetDU);
            this.groupBoxMessage.Controls.Add(this.clbTargetDU);
            this.groupBoxMessage.Controls.Add(this.label7);
            this.groupBoxMessage.Controls.Add(this.btnRefreshMessageID);
            this.groupBoxMessage.Controls.Add(this.cboMessageIDEn);
            this.groupBoxMessage.Controls.Add(this.label6En);
            this.groupBoxMessage.Controls.Add(this.cboMessageID);
            this.groupBoxMessage.Controls.Add(this.label6);
            this.groupBoxMessage.Controls.Add(this.txtSeatID);
            this.groupBoxMessage.Controls.Add(this.label5);
            this.groupBoxMessage.Controls.Add(this.cboStation);
            this.groupBoxMessage.Controls.Add(this.label4);
            this.groupBoxMessage.Controls.Add(this.cboArea);
            this.groupBoxMessage.Controls.Add(this.labelArea);
            this.groupBoxMessage.Controls.Add(this.cboMessageType);
            this.groupBoxMessage.Controls.Add(this.labelMessageType);
            this.groupBoxMessage.Controls.Add(this.cboPriority);
            this.groupBoxMessage.Controls.Add(this.labelPriority);
            this.groupBoxMessage.Controls.Add(this.cboMoveSpeed);
            this.groupBoxMessage.Controls.Add(this.labelMoveSpeed);
            this.groupBoxMessage.Controls.Add(this.cboMoveMode);
            this.groupBoxMessage.Controls.Add(this.labelMoveMode);
            this.groupBoxMessage.Location = new System.Drawing.Point(15, 235);
            this.groupBoxMessage.Name = "groupBoxMessage";
            this.groupBoxMessage.Size = new System.Drawing.Size(720, 389);
            this.groupBoxMessage.TabIndex = 2;
            this.groupBoxMessage.TabStop = false;
            this.groupBoxMessage.Text = "訊息設定";
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.Location = new System.Drawing.Point(550, 260);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(160, 35);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "發送訊息";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnRefreshTargetDU
            // 
            this.btnRefreshTargetDU.Location = new System.Drawing.Point(650, 208);
            this.btnRefreshTargetDU.Name = "btnRefreshTargetDU";
            this.btnRefreshTargetDU.Size = new System.Drawing.Size(60, 24);
            this.btnRefreshTargetDU.TabIndex = 10;
            this.btnRefreshTargetDU.Text = "刷新";
            this.btnRefreshTargetDU.UseVisualStyleBackColor = true;
            this.btnRefreshTargetDU.Click += new System.EventHandler(this.btnRefreshTargetDU_Click);
            // 
            // clbTargetDU
            // 
            this.clbTargetDU.CheckBoxes = true;
            this.clbTargetDU.FullRowSelect = true;
            this.clbTargetDU.GridLines = true;
            this.clbTargetDU.HideSelection = false;
            this.clbTargetDU.Location = new System.Drawing.Point(120, 260);
            this.clbTargetDU.Name = "clbTargetDU";
            this.clbTargetDU.Size = new System.Drawing.Size(412, 123);
            this.clbTargetDU.TabIndex = 9;
            this.clbTargetDU.UseCompatibleStateImageBehavior = false;
            this.clbTargetDU.View = System.Windows.Forms.View.Details;
            this.clbTargetDU.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbTargetDU_ItemCheck);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 260);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "目標看板:";
            // 
            // btnRefreshMessageID
            // 
            this.btnRefreshMessageID.Location = new System.Drawing.Point(650, 143);
            this.btnRefreshMessageID.Name = "btnRefreshMessageID";
            this.btnRefreshMessageID.Size = new System.Drawing.Size(60, 24);
            this.btnRefreshMessageID.TabIndex = 8;
            this.btnRefreshMessageID.Text = "刷新";
            this.btnRefreshMessageID.UseVisualStyleBackColor = true;
            this.btnRefreshMessageID.Click += new System.EventHandler(this.btnRefreshMessageID_Click);
            // 
            // cboMessageIDEn
            // 
            this.cboMessageIDEn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageIDEn.FormattingEnabled = true;
            this.cboMessageIDEn.Location = new System.Drawing.Point(130, 205);
            this.cboMessageIDEn.Name = "cboMessageIDEn";
            this.cboMessageIDEn.Size = new System.Drawing.Size(500, 20);
            this.cboMessageIDEn.TabIndex = 9;
            this.cboMessageIDEn.SelectedIndexChanged += new System.EventHandler(this.cboMessageIDEn_SelectedIndexChanged);
            // 
            // label6En
            // 
            this.label6En.AutoSize = true;
            this.label6En.Location = new System.Drawing.Point(20, 208);
            this.label6En.Name = "label6En";
            this.label6En.Size = new System.Drawing.Size(56, 12);
            this.label6En.TabIndex = 10;
            this.label6En.Text = "英文訊息:";
            // 
            // cboMessageID
            // 
            this.cboMessageID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageID.FormattingEnabled = true;
            this.cboMessageID.Location = new System.Drawing.Point(130, 140);
            this.cboMessageID.Name = "cboMessageID";
            this.cboMessageID.Size = new System.Drawing.Size(500, 20);
            this.cboMessageID.TabIndex = 7;
            this.cboMessageID.SelectedIndexChanged += new System.EventHandler(this.cboMessageID_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "中文訊息:";
            // 
            // txtSeatID
            // 
            this.txtSeatID.Location = new System.Drawing.Point(130, 105);
            this.txtSeatID.Name = "txtSeatID";
            this.txtSeatID.Size = new System.Drawing.Size(200, 22);
            this.txtSeatID.TabIndex = 5;
            this.txtSeatID.Text = "DMD";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Seat ID:";
            // 
            // cboStation
            // 
            this.cboStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStation.FormattingEnabled = true;
            this.cboStation.Location = new System.Drawing.Point(480, 30);
            this.cboStation.Name = "cboStation";
            this.cboStation.Size = new System.Drawing.Size(150, 20);
            this.cboStation.TabIndex = 3;
            this.cboStation.SelectedIndexChanged += new System.EventHandler(this.cboStation_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(410, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "車站:";
            // 
            // cboArea
            // 
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(480, 60);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(150, 20);
            this.cboArea.TabIndex = 21;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // labelArea
            // 
            this.labelArea.AutoSize = true;
            this.labelArea.Location = new System.Drawing.Point(410, 63);
            this.labelArea.Name = "labelArea";
            this.labelArea.Size = new System.Drawing.Size(32, 12);
            this.labelArea.TabIndex = 20;
            this.labelArea.Text = "區域:";
            // 
            // cboMessageType
            // 
            this.cboMessageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageType.FormattingEnabled = true;
            this.cboMessageType.Location = new System.Drawing.Point(130, 30);
            this.cboMessageType.Name = "cboMessageType";
            this.cboMessageType.Size = new System.Drawing.Size(260, 20);
            this.cboMessageType.TabIndex = 1;
            // 
            // labelMessageType
            // 
            this.labelMessageType.AutoSize = true;
            this.labelMessageType.Location = new System.Drawing.Point(20, 33);
            this.labelMessageType.Name = "labelMessageType";
            this.labelMessageType.Size = new System.Drawing.Size(56, 12);
            this.labelMessageType.TabIndex = 0;
            this.labelMessageType.Text = "訊息類型:";
            // 
            // cboPriority
            // 
            this.cboPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPriority.FormattingEnabled = true;
            this.cboPriority.Location = new System.Drawing.Point(130, 235);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(100, 20);
            this.cboPriority.TabIndex = 11;
            // 
            // labelPriority
            // 
            this.labelPriority.AutoSize = true;
            this.labelPriority.Location = new System.Drawing.Point(20, 238);
            this.labelPriority.Name = "labelPriority";
            this.labelPriority.Size = new System.Drawing.Size(56, 12);
            this.labelPriority.TabIndex = 10;
            this.labelPriority.Text = "優先等級:";
            // 
            // cboMoveSpeed
            // 
            this.cboMoveSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoveSpeed.FormattingEnabled = true;
            this.cboMoveSpeed.Location = new System.Drawing.Point(340, 235);
            this.cboMoveSpeed.Name = "cboMoveSpeed";
            this.cboMoveSpeed.Size = new System.Drawing.Size(100, 20);
            this.cboMoveSpeed.TabIndex = 13;
            // 
            // labelMoveSpeed
            // 
            this.labelMoveSpeed.AutoSize = true;
            this.labelMoveSpeed.Location = new System.Drawing.Point(270, 238);
            this.labelMoveSpeed.Name = "labelMoveSpeed";
            this.labelMoveSpeed.Size = new System.Drawing.Size(56, 12);
            this.labelMoveSpeed.TabIndex = 12;
            this.labelMoveSpeed.Text = "移動速度:";
            // 
            // cboMoveMode
            // 
            this.cboMoveMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoveMode.FormattingEnabled = true;
            this.cboMoveMode.Location = new System.Drawing.Point(530, 235);
            this.cboMoveMode.Name = "cboMoveMode";
            this.cboMoveMode.Size = new System.Drawing.Size(100, 20);
            this.cboMoveMode.TabIndex = 15;
            // 
            // labelMoveMode
            // 
            this.labelMoveMode.AutoSize = true;
            this.labelMoveMode.Location = new System.Drawing.Point(460, 238);
            this.labelMoveMode.Name = "labelMoveMode";
            this.labelMoveMode.Size = new System.Drawing.Size(56, 12);
            this.labelMoveMode.TabIndex = 14;
            this.labelMoveMode.Text = "移動方式:";
            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Controls.Add(this.btnExportLog);
            this.groupBoxLog.Controls.Add(this.btnClearLog);
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Location = new System.Drawing.Point(751, 15);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(593, 750);
            this.groupBoxLog.TabIndex = 5;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "日誌";
            // 
            // btnExportLog
            // 
            this.btnExportLog.Location = new System.Drawing.Point(130, 722);
            this.btnExportLog.Name = "btnExportLog";
            this.btnExportLog.Size = new System.Drawing.Size(80, 25);
            this.btnExportLog.TabIndex = 2;
            this.btnExportLog.Text = "匯出";
            this.btnExportLog.UseVisualStyleBackColor = true;
            this.btnExportLog.Click += new System.EventHandler(this.btnExportLog_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(44, 723);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(80, 25);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "清除";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.Location = new System.Drawing.Point(20, 33);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(405, 683);
            this.txtLog.TabIndex = 0;
            // 
            // SendToDCU
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.Controls.Add(this.groupBoxLog);
            this.Controls.Add(this.groupBoxInstantMessage);
            this.Controls.Add(this.groupBoxMessage);
            this.Controls.Add(this.groupBoxDatabase);
            this.Controls.Add(this.groupBoxConnection);
            this.Name = "SendToDCU";
            this.Size = new System.Drawing.Size(1355, 820);
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxDatabase.ResumeLayout(false);
            this.groupBoxDatabase.PerformLayout();
            this.groupBoxInstantMessage.ResumeLayout(false);
            this.groupBoxInstantMessage.PerformLayout();
            this.groupBoxMessage.ResumeLayout(false);
            this.groupBoxMessage.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.GroupBox groupBoxDatabase;
        private System.Windows.Forms.TextBox txtDBIP;
        private System.Windows.Forms.TextBox txtDBPort;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.TextBox txtDBUserID;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.Button btnConnectDB;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBoxMessage;
        private System.Windows.Forms.Label labelMessageType;
        private System.Windows.Forms.ComboBox cboMessageType;
        private System.Windows.Forms.ComboBox cboStation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.Label labelArea;
        private System.Windows.Forms.TextBox txtSeatID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboMessageID;
        private System.Windows.Forms.Button btnRefreshMessageID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboMessageIDEn;
        private System.Windows.Forms.Label label6En;
        private System.Windows.Forms.ListView clbTargetDU;
        private System.Windows.Forms.Button btnRefreshTargetDU;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnExportLog;
        private System.Windows.Forms.ComboBox cboPriority;
        private System.Windows.Forms.Label labelPriority;
        private System.Windows.Forms.ComboBox cboMoveSpeed;
        private System.Windows.Forms.Label labelMoveSpeed;
        private System.Windows.Forms.ComboBox cboMoveMode;
        private System.Windows.Forms.Label labelMoveMode;
        private System.Windows.Forms.GroupBox groupBoxInstantMessage;
        private System.Windows.Forms.TextBox txtInstantMessageCHN;
        private System.Windows.Forms.TextBox txtInstantMessageENG;
        private System.Windows.Forms.Button btnLoadInstantMessage;
        private System.Windows.Forms.Button btnSaveInstantMessage;
        private System.Windows.Forms.Label labelInstantMessageCHN;
        private System.Windows.Forms.Label labelInstantMessageENG;
    }
}
