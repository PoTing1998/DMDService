namespace UITest
{
    partial class TaskDCU
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.testBtn = new System.Windows.Forms.Button();
            this.stationListCB = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DCUdataGridView = new System.Windows.Forms.DataGridView();
            this.DMDdataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxTYPE = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonDispose = new System.Windows.Forms.Button();
            this.textBoxConnPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxConnIP = new System.Windows.Forms.TextBox();
            this.buttonInit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DCUdataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DMDdataGridView)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(359, 147);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(105, 24);
            this.button3.TabIndex = 58;
            this.button3.Text = "傳送電源";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(303, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 110);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(175, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(61, 22);
            this.textBox1.TabIndex = 30;
            this.textBox1.Text = "Server";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(173, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "Type";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(100, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 28;
            this.button1.Text = "Dispose";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(100, 41);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(61, 22);
            this.textBox2.TabIndex = 27;
            this.textBox2.Text = "2000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(98, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "port";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "DMD Server IP";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(8, 41);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(75, 22);
            this.textBox3.TabIndex = 24;
            this.textBox3.Text = "10.107.26.55";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(8, 69);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 23;
            this.button2.Text = "Initial";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // testBtn
            // 
            this.testBtn.Location = new System.Drawing.Point(235, 147);
            this.testBtn.Name = "testBtn";
            this.testBtn.Size = new System.Drawing.Size(105, 24);
            this.testBtn.TabIndex = 56;
            this.testBtn.Text = "傳送測試";
            this.testBtn.UseVisualStyleBackColor = true;
            // 
            // stationListCB
            // 
            this.stationListCB.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.stationListCB.FormattingEnabled = true;
            this.stationListCB.Location = new System.Drawing.Point(511, 159);
            this.stationListCB.Name = "stationListCB";
            this.stationListCB.Size = new System.Drawing.Size(229, 29);
            this.stationListCB.TabIndex = 55;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(587, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 19);
            this.label2.TabIndex = 54;
            this.label2.Text = "車站端設備清單";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(68, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 19);
            this.label1.TabIndex = 53;
            this.label1.Text = "行控中心設備清單";
            // 
            // DCUdataGridView
            // 
            this.DCUdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DCUdataGridView.Location = new System.Drawing.Point(511, 194);
            this.DCUdataGridView.Name = "DCUdataGridView";
            this.DCUdataGridView.RowTemplate.Height = 24;
            this.DCUdataGridView.Size = new System.Drawing.Size(352, 551);
            this.DCUdataGridView.TabIndex = 52;
            // 
            // DMDdataGridView
            // 
            this.DMDdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DMDdataGridView.Location = new System.Drawing.Point(57, 194);
            this.DMDdataGridView.Name = "DMDdataGridView";
            this.DMDdataGridView.RowTemplate.Height = 24;
            this.DMDdataGridView.Size = new System.Drawing.Size(265, 551);
            this.DMDdataGridView.TabIndex = 51;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxTYPE);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.buttonDispose);
            this.groupBox3.Controls.Add(this.textBoxConnPort);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxConnIP);
            this.groupBox3.Controls.Add(this.buttonInit);
            this.groupBox3.Location = new System.Drawing.Point(27, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(245, 110);
            this.groupBox3.TabIndex = 50;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "設定";
            // 
            // textBoxTYPE
            // 
            this.textBoxTYPE.Location = new System.Drawing.Point(175, 41);
            this.textBoxTYPE.Name = "textBoxTYPE";
            this.textBoxTYPE.Size = new System.Drawing.Size(61, 22);
            this.textBoxTYPE.TabIndex = 30;
            this.textBoxTYPE.Text = "Server";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "Type";
            // 
            // buttonDispose
            // 
            this.buttonDispose.Location = new System.Drawing.Point(100, 69);
            this.buttonDispose.Name = "buttonDispose";
            this.buttonDispose.Size = new System.Drawing.Size(75, 23);
            this.buttonDispose.TabIndex = 28;
            this.buttonDispose.Text = "Dispose";
            this.buttonDispose.UseVisualStyleBackColor = true;
            // 
            // textBoxConnPort
            // 
            this.textBoxConnPort.Location = new System.Drawing.Point(100, 41);
            this.textBoxConnPort.Name = "textBoxConnPort";
            this.textBoxConnPort.Size = new System.Drawing.Size(61, 22);
            this.textBoxConnPort.TabIndex = 27;
            this.textBoxConnPort.Text = "8000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(98, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "DMD Server IP";
            // 
            // textBoxConnIP
            // 
            this.textBoxConnIP.Location = new System.Drawing.Point(8, 41);
            this.textBoxConnIP.Name = "textBoxConnIP";
            this.textBoxConnIP.Size = new System.Drawing.Size(75, 22);
            this.textBoxConnIP.TabIndex = 24;
            this.textBoxConnIP.Text = "10.107.26.99";
            // 
            // buttonInit
            // 
            this.buttonInit.Location = new System.Drawing.Point(8, 69);
            this.buttonInit.Name = "buttonInit";
            this.buttonInit.Size = new System.Drawing.Size(75, 23);
            this.buttonInit.TabIndex = 23;
            this.buttonInit.Text = "Initial";
            this.buttonInit.UseVisualStyleBackColor = true;
            // 
            // TaskDCU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.testBtn);
            this.Controls.Add(this.stationListCB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DCUdataGridView);
            this.Controls.Add(this.DMDdataGridView);
            this.Controls.Add(this.groupBox3);
            this.Name = "TaskDCU";
            this.Size = new System.Drawing.Size(951, 827);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DCUdataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DMDdataGridView)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button testBtn;
        private System.Windows.Forms.ComboBox stationListCB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView DCUdataGridView;
        private System.Windows.Forms.DataGridView DMDdataGridView;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxTYPE;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonDispose;
        private System.Windows.Forms.TextBox textBoxConnPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxConnIP;
        private System.Windows.Forms.Button buttonInit;
    }
}
