namespace UITest
{
    partial class SocketMonitor
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblDcuStatus = new System.Windows.Forms.Label();
            this.lblCmftStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.numDcuPort = new System.Windows.Forms.NumericUpDown();
            this.chkDcu = new System.Windows.Forms.CheckBox();
            this.numCmftPort = new System.Windows.Forms.NumericUpDown();
            this.chkCmft = new System.Windows.Forms.CheckBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tabView = new System.Windows.Forms.TabControl();
            this.tabStations = new System.Windows.Forms.TabPage();
            this.gridStations = new System.Windows.Forms.DataGridView();
            this.panelStationButtons = new System.Windows.Forms.Panel();
            this.lblStationSummary = new System.Windows.Forms.Label();
            this.btnKickStation = new System.Windows.Forms.Button();
            this.tabAll = new System.Windows.Forms.TabPage();
            this.gridClients = new System.Windows.Forms.DataGridView();
            this.panelClientButtons = new System.Windows.Forms.Panel();
            this.lblClientCount = new System.Windows.Forms.Label();
            this.btnKick = new System.Windows.Forms.Button();
            this.splitBottom = new System.Windows.Forms.SplitContainer();
            this.grpSend = new System.Windows.Forms.GroupBox();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.panelSendOptions = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.numMsgId = new System.Windows.Forms.NumericUpDown();
            this.lblMsgId = new System.Windows.Forms.Label();
            this.cboMsgType = new System.Windows.Forms.ComboBox();
            this.lblMsgType = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.grpEvents = new System.Windows.Forms.GroupBox();
            this.gridEvents = new System.Windows.Forms.DataGridView();
            this.panelEventButtons = new System.Windows.Forms.Panel();
            this.btnClearEvents = new System.Windows.Forms.Button();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDcuPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCmftPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabView.SuspendLayout();
            this.tabStations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridStations)).BeginInit();
            this.panelStationButtons.SuspendLayout();
            this.tabAll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).BeginInit();
            this.panelClientButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitBottom)).BeginInit();
            this.splitBottom.Panel1.SuspendLayout();
            this.splitBottom.Panel2.SuspendLayout();
            this.splitBottom.SuspendLayout();
            this.grpSend.SuspendLayout();
            this.panelSendOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMsgId)).BeginInit();
            this.grpEvents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEvents)).BeginInit();
            this.panelEventButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblDcuStatus);
            this.panelTop.Controls.Add(this.lblCmftStatus);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.numInterval);
            this.panelTop.Controls.Add(this.lblInterval);
            this.panelTop.Controls.Add(this.chkAutoRefresh);
            this.panelTop.Controls.Add(this.numDcuPort);
            this.panelTop.Controls.Add(this.chkDcu);
            this.panelTop.Controls.Add(this.numCmftPort);
            this.panelTop.Controls.Add(this.chkCmft);
            this.panelTop.Controls.Add(this.txtHost);
            this.panelTop.Controls.Add(this.lblHost);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(6);
            this.panelTop.Size = new System.Drawing.Size(1300, 84);
            this.panelTop.TabIndex = 0;
            //
            // lblHost
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(10, 14);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(64, 17);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "服務主機";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(80, 10);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(130, 25);
            this.txtHost.TabIndex = 1;
            this.txtHost.Text = "127.0.0.1";
            //
            // chkCmft
            //
            this.chkCmft.AutoSize = true;
            this.chkCmft.Checked = true;
            this.chkCmft.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCmft.Location = new System.Drawing.Point(230, 12);
            this.chkCmft.Name = "chkCmft";
            this.chkCmft.Size = new System.Drawing.Size(122, 21);
            this.chkCmft.TabIndex = 2;
            this.chkCmft.Text = "TaskCMFT 管理埠";
            this.chkCmft.UseVisualStyleBackColor = true;
            //
            // numCmftPort
            //
            this.numCmftPort.Location = new System.Drawing.Point(356, 10);
            this.numCmftPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numCmftPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCmftPort.Name = "numCmftPort";
            this.numCmftPort.Size = new System.Drawing.Size(80, 25);
            this.numCmftPort.TabIndex = 3;
            this.numCmftPort.Value = new decimal(new int[] { 18000, 0, 0, 0 });
            //
            // chkDcu
            //
            this.chkDcu.AutoSize = true;
            this.chkDcu.Checked = true;
            this.chkDcu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDcu.Location = new System.Drawing.Point(456, 12);
            this.chkDcu.Name = "chkDcu";
            this.chkDcu.Size = new System.Drawing.Size(115, 21);
            this.chkDcu.TabIndex = 4;
            this.chkDcu.Text = "TaskDCU 管理埠";
            this.chkDcu.UseVisualStyleBackColor = true;
            //
            // numDcuPort
            //
            this.numDcuPort.Location = new System.Drawing.Point(576, 10);
            this.numDcuPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numDcuPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numDcuPort.Name = "numDcuPort";
            this.numDcuPort.Size = new System.Drawing.Size(80, 25);
            this.numDcuPort.TabIndex = 5;
            this.numDcuPort.Value = new decimal(new int[] { 18001, 0, 0, 0 });
            //
            // chkAutoRefresh
            //
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Checked = true;
            this.chkAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRefresh.Location = new System.Drawing.Point(680, 12);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(79, 21);
            this.chkAutoRefresh.TabIndex = 6;
            this.chkAutoRefresh.Text = "自動刷新";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            this.chkAutoRefresh.CheckedChanged += new System.EventHandler(this.chkAutoRefresh_CheckedChanged);
            //
            // lblInterval
            //
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(766, 14);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(34, 17);
            this.lblInterval.TabIndex = 7;
            this.lblInterval.Text = "間隔";
            //
            // numInterval
            //
            this.numInterval.Location = new System.Drawing.Point(804, 10);
            this.numInterval.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.numInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(50, 25);
            this.numInterval.TabIndex = 8;
            this.numInterval.Value = new decimal(new int[] { 2, 0, 0, 0 });
            this.numInterval.ValueChanged += new System.EventHandler(this.numInterval_ValueChanged);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(880, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 29);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "立即刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblCmftStatus
            //
            this.lblCmftStatus.AutoSize = true;
            this.lblCmftStatus.Location = new System.Drawing.Point(10, 48);
            this.lblCmftStatus.Name = "lblCmftStatus";
            this.lblCmftStatus.Size = new System.Drawing.Size(90, 17);
            this.lblCmftStatus.TabIndex = 10;
            this.lblCmftStatus.Text = "CMFT：尚未查詢";
            //
            // lblDcuStatus
            //
            this.lblDcuStatus.AutoSize = true;
            this.lblDcuStatus.Location = new System.Drawing.Point(640, 48);
            this.lblDcuStatus.Name = "lblDcuStatus";
            this.lblDcuStatus.Size = new System.Drawing.Size(84, 17);
            this.lblDcuStatus.TabIndex = 11;
            this.lblDcuStatus.Text = "DCU：尚未查詢";
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 84);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitMain.Panel1
            //
            this.splitMain.Panel1.Controls.Add(this.tabView);
            //
            // splitMain.Panel2
            //
            this.splitMain.Panel2.Controls.Add(this.splitBottom);
            this.splitMain.Size = new System.Drawing.Size(1300, 716);
            this.splitMain.SplitterDistance = 400;
            this.splitMain.TabIndex = 1;
            //
            // tabView
            //
            this.tabView.Controls.Add(this.tabStations);
            this.tabView.Controls.Add(this.tabAll);
            this.tabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabView.Location = new System.Drawing.Point(0, 0);
            this.tabView.Name = "tabView";
            this.tabView.SelectedIndex = 0;
            this.tabView.Size = new System.Drawing.Size(1300, 400);
            this.tabView.TabIndex = 0;
            this.tabView.SelectedIndexChanged += new System.EventHandler(this.tabView_SelectedIndexChanged);
            //
            // tabStations
            //
            this.tabStations.Controls.Add(this.gridStations);
            this.tabStations.Controls.Add(this.panelStationButtons);
            this.tabStations.Location = new System.Drawing.Point(4, 26);
            this.tabStations.Name = "tabStations";
            this.tabStations.Size = new System.Drawing.Size(1292, 370);
            this.tabStations.TabIndex = 0;
            this.tabStations.Text = "DCU 車站總覽";
            this.tabStations.UseVisualStyleBackColor = true;
            //
            // gridStations
            //
            this.gridStations.AllowUserToAddRows = false;
            this.gridStations.AllowUserToDeleteRows = false;
            this.gridStations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridStations.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridStations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridStations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridStations.Location = new System.Drawing.Point(0, 0);
            this.gridStations.MultiSelect = false;
            this.gridStations.Name = "gridStations";
            this.gridStations.ReadOnly = true;
            this.gridStations.RowHeadersVisible = false;
            this.gridStations.RowTemplate.Height = 28;
            this.gridStations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridStations.Size = new System.Drawing.Size(1292, 330);
            this.gridStations.TabIndex = 0;
            this.gridStations.SelectionChanged += new System.EventHandler(this.gridClients_SelectionChanged);
            //
            // panelStationButtons
            //
            this.panelStationButtons.Controls.Add(this.lblStationSummary);
            this.panelStationButtons.Controls.Add(this.btnKickStation);
            this.panelStationButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStationButtons.Location = new System.Drawing.Point(0, 330);
            this.panelStationButtons.Name = "panelStationButtons";
            this.panelStationButtons.Size = new System.Drawing.Size(1292, 40);
            this.panelStationButtons.TabIndex = 1;
            //
            // btnKickStation
            //
            this.btnKickStation.Enabled = false;
            this.btnKickStation.ForeColor = System.Drawing.Color.Firebrick;
            this.btnKickStation.Location = new System.Drawing.Point(6, 5);
            this.btnKickStation.Name = "btnKickStation";
            this.btnKickStation.Size = new System.Drawing.Size(160, 30);
            this.btnKickStation.TabIndex = 0;
            this.btnKickStation.Text = "斷開此站所有連線";
            this.btnKickStation.UseVisualStyleBackColor = true;
            this.btnKickStation.Click += new System.EventHandler(this.btnKickStation_Click);
            //
            // lblStationSummary
            //
            this.lblStationSummary.AutoSize = true;
            this.lblStationSummary.Location = new System.Drawing.Point(180, 11);
            this.lblStationSummary.Name = "lblStationSummary";
            this.lblStationSummary.Size = new System.Drawing.Size(120, 17);
            this.lblStationSummary.TabIndex = 1;
            this.lblStationSummary.Text = "DCU 車站：尚未查詢";
            //
            // tabAll
            //
            this.tabAll.Controls.Add(this.gridClients);
            this.tabAll.Controls.Add(this.panelClientButtons);
            this.tabAll.Location = new System.Drawing.Point(4, 26);
            this.tabAll.Name = "tabAll";
            this.tabAll.Size = new System.Drawing.Size(1292, 370);
            this.tabAll.TabIndex = 1;
            this.tabAll.Text = "全部連線";
            this.tabAll.UseVisualStyleBackColor = true;
            //
            // gridClients
            //
            this.gridClients.AllowUserToAddRows = false;
            this.gridClients.AllowUserToDeleteRows = false;
            this.gridClients.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridClients.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridClients.Location = new System.Drawing.Point(0, 0);
            this.gridClients.MultiSelect = false;
            this.gridClients.Name = "gridClients";
            this.gridClients.ReadOnly = true;
            this.gridClients.RowHeadersVisible = false;
            this.gridClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridClients.Size = new System.Drawing.Size(1300, 320);
            this.gridClients.TabIndex = 0;
            this.gridClients.SelectionChanged += new System.EventHandler(this.gridClients_SelectionChanged);
            //
            // panelClientButtons
            //
            this.panelClientButtons.Controls.Add(this.lblClientCount);
            this.panelClientButtons.Controls.Add(this.btnKick);
            this.panelClientButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelClientButtons.Location = new System.Drawing.Point(0, 320);
            this.panelClientButtons.Name = "panelClientButtons";
            this.panelClientButtons.Size = new System.Drawing.Size(1300, 40);
            this.panelClientButtons.TabIndex = 1;
            //
            // btnKick
            //
            this.btnKick.Enabled = false;
            this.btnKick.ForeColor = System.Drawing.Color.Firebrick;
            this.btnKick.Location = new System.Drawing.Point(6, 5);
            this.btnKick.Name = "btnKick";
            this.btnKick.Size = new System.Drawing.Size(140, 30);
            this.btnKick.TabIndex = 0;
            this.btnKick.Text = "強制斷開選取連線";
            this.btnKick.UseVisualStyleBackColor = true;
            this.btnKick.Click += new System.EventHandler(this.btnKick_Click);
            //
            // lblClientCount
            //
            this.lblClientCount.AutoSize = true;
            this.lblClientCount.Location = new System.Drawing.Point(160, 11);
            this.lblClientCount.Name = "lblClientCount";
            this.lblClientCount.Size = new System.Drawing.Size(80, 17);
            this.lblClientCount.TabIndex = 1;
            this.lblClientCount.Text = "目前連線：0";
            //
            // splitBottom
            //
            this.splitBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBottom.Location = new System.Drawing.Point(0, 0);
            this.splitBottom.Name = "splitBottom";
            //
            // splitBottom.Panel1
            //
            this.splitBottom.Panel1.Controls.Add(this.grpSend);
            //
            // splitBottom.Panel2
            //
            this.splitBottom.Panel2.Controls.Add(this.grpEvents);
            this.splitBottom.Size = new System.Drawing.Size(1300, 352);
            this.splitBottom.SplitterDistance = 480;
            this.splitBottom.TabIndex = 0;
            //
            // grpSend
            //
            this.grpSend.Controls.Add(this.txtJson);
            this.grpSend.Controls.Add(this.panelSendOptions);
            this.grpSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSend.Location = new System.Drawing.Point(0, 0);
            this.grpSend.Name = "grpSend";
            this.grpSend.Size = new System.Drawing.Size(480, 352);
            this.grpSend.TabIndex = 0;
            this.grpSend.TabStop = false;
            this.grpSend.Text = "對選取的 Client 發送測試訊息";
            //
            // panelSendOptions
            //
            this.panelSendOptions.Controls.Add(this.btnSend);
            this.panelSendOptions.Controls.Add(this.numMsgId);
            this.panelSendOptions.Controls.Add(this.lblMsgId);
            this.panelSendOptions.Controls.Add(this.cboMsgType);
            this.panelSendOptions.Controls.Add(this.lblMsgType);
            this.panelSendOptions.Controls.Add(this.lblTarget);
            this.panelSendOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSendOptions.Location = new System.Drawing.Point(3, 21);
            this.panelSendOptions.Name = "panelSendOptions";
            this.panelSendOptions.Size = new System.Drawing.Size(474, 70);
            this.panelSendOptions.TabIndex = 0;
            //
            // lblTarget
            //
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(4, 6);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(116, 17);
            this.lblTarget.TabIndex = 0;
            this.lblTarget.Text = "目標：(未選取連線)";
            //
            // lblMsgType
            //
            this.lblMsgType.AutoSize = true;
            this.lblMsgType.Location = new System.Drawing.Point(4, 40);
            this.lblMsgType.Name = "lblMsgType";
            this.lblMsgType.Size = new System.Drawing.Size(60, 17);
            this.lblMsgType.TabIndex = 1;
            this.lblMsgType.Text = "訊息類別";
            //
            // cboMsgType
            //
            this.cboMsgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMsgType.Location = new System.Drawing.Point(68, 36);
            this.cboMsgType.Name = "cboMsgType";
            this.cboMsgType.Size = new System.Drawing.Size(130, 25);
            this.cboMsgType.TabIndex = 2;
            //
            // lblMsgId
            //
            this.lblMsgId.AutoSize = true;
            this.lblMsgId.Location = new System.Drawing.Point(208, 40);
            this.lblMsgId.Name = "lblMsgId";
            this.lblMsgId.Size = new System.Drawing.Size(47, 17);
            this.lblMsgId.TabIndex = 3;
            this.lblMsgId.Text = "識別碼";
            //
            // numMsgId
            //
            this.numMsgId.Location = new System.Drawing.Point(260, 36);
            this.numMsgId.Maximum = new decimal(new int[] { 2147483647, 0, 0, 0 });
            this.numMsgId.Name = "numMsgId";
            this.numMsgId.Size = new System.Drawing.Size(90, 25);
            this.numMsgId.TabIndex = 4;
            this.numMsgId.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btnSend
            //
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(366, 34);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 29);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "發送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            //
            // txtJson
            //
            this.txtJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJson.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtJson.Location = new System.Drawing.Point(3, 91);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJson.Size = new System.Drawing.Size(474, 258);
            this.txtJson.TabIndex = 1;
            this.txtJson.WordWrap = false;
            //
            // grpEvents
            //
            this.grpEvents.Controls.Add(this.gridEvents);
            this.grpEvents.Controls.Add(this.panelEventButtons);
            this.grpEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEvents.Location = new System.Drawing.Point(0, 0);
            this.grpEvents.Name = "grpEvents";
            this.grpEvents.Size = new System.Drawing.Size(816, 352);
            this.grpEvents.TabIndex = 0;
            this.grpEvents.TabStop = false;
            this.grpEvents.Text = "連線 / 斷線事件紀錄";
            //
            // gridEvents
            //
            this.gridEvents.AllowUserToAddRows = false;
            this.gridEvents.AllowUserToDeleteRows = false;
            this.gridEvents.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEvents.Location = new System.Drawing.Point(3, 21);
            this.gridEvents.Name = "gridEvents";
            this.gridEvents.ReadOnly = true;
            this.gridEvents.RowHeadersVisible = false;
            this.gridEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridEvents.Size = new System.Drawing.Size(810, 292);
            this.gridEvents.TabIndex = 0;
            //
            // panelEventButtons
            //
            this.panelEventButtons.Controls.Add(this.btnClearEvents);
            this.panelEventButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelEventButtons.Location = new System.Drawing.Point(3, 313);
            this.panelEventButtons.Name = "panelEventButtons";
            this.panelEventButtons.Size = new System.Drawing.Size(810, 36);
            this.panelEventButtons.TabIndex = 1;
            //
            // btnClearEvents
            //
            this.btnClearEvents.Location = new System.Drawing.Point(4, 4);
            this.btnClearEvents.Name = "btnClearEvents";
            this.btnClearEvents.Size = new System.Drawing.Size(100, 28);
            this.btnClearEvents.TabIndex = 0;
            this.btnClearEvents.Text = "清除畫面";
            this.btnClearEvents.UseVisualStyleBackColor = true;
            this.btnClearEvents.Click += new System.EventHandler(this.btnClearEvents_Click);
            //
            // refreshTimer
            //
            this.refreshTimer.Interval = 2000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            //
            // SocketMonitor
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "SocketMonitor";
            this.Size = new System.Drawing.Size(1300, 800);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDcuPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCmftPort)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.tabView.ResumeLayout(false);
            this.tabStations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridStations)).EndInit();
            this.panelStationButtons.ResumeLayout(false);
            this.panelStationButtons.PerformLayout();
            this.tabAll.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).EndInit();
            this.panelClientButtons.ResumeLayout(false);
            this.panelClientButtons.PerformLayout();
            this.splitBottom.Panel1.ResumeLayout(false);
            this.splitBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBottom)).EndInit();
            this.splitBottom.ResumeLayout(false);
            this.grpSend.ResumeLayout(false);
            this.grpSend.PerformLayout();
            this.panelSendOptions.ResumeLayout(false);
            this.panelSendOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMsgId)).EndInit();
            this.grpEvents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridEvents)).EndInit();
            this.panelEventButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.CheckBox chkCmft;
        private System.Windows.Forms.NumericUpDown numCmftPort;
        private System.Windows.Forms.CheckBox chkDcu;
        private System.Windows.Forms.NumericUpDown numDcuPort;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblCmftStatus;
        private System.Windows.Forms.Label lblDcuStatus;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabView;
        private System.Windows.Forms.TabPage tabStations;
        private System.Windows.Forms.DataGridView gridStations;
        private System.Windows.Forms.Panel panelStationButtons;
        private System.Windows.Forms.Label lblStationSummary;
        private System.Windows.Forms.Button btnKickStation;
        private System.Windows.Forms.TabPage tabAll;
        private System.Windows.Forms.DataGridView gridClients;
        private System.Windows.Forms.Panel panelClientButtons;
        private System.Windows.Forms.Button btnKick;
        private System.Windows.Forms.Label lblClientCount;
        private System.Windows.Forms.SplitContainer splitBottom;
        private System.Windows.Forms.GroupBox grpSend;
        private System.Windows.Forms.Panel panelSendOptions;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblMsgType;
        private System.Windows.Forms.ComboBox cboMsgType;
        private System.Windows.Forms.Label lblMsgId;
        private System.Windows.Forms.NumericUpDown numMsgId;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.GroupBox grpEvents;
        private System.Windows.Forms.DataGridView gridEvents;
        private System.Windows.Forms.Panel panelEventButtons;
        private System.Windows.Forms.Button btnClearEvents;
        private System.Windows.Forms.Timer refreshTimer;
    }
}
