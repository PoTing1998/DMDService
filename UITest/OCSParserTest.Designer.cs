namespace UITest
{
    partial class OCSParserTest
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            var defaultFont = new System.Drawing.Font("Microsoft JhengHei", 9F);

            this.grpInput    = new System.Windows.Forms.GroupBox();
            this.lblHint     = new System.Windows.Forms.Label();
            this.txtInput    = new System.Windows.Forms.TextBox();
            this.btnSample   = new System.Windows.Forms.Button();
            this.btnParse    = new System.Windows.Forms.Button();
            this.btnClear    = new System.Windows.Forms.Button();
            this.lblSummary  = new System.Windows.Forms.Label();
            this.listResults = new System.Windows.Forms.ListView();
            this.colField    = new System.Windows.Forms.ColumnHeader();
            this.colByte     = new System.Windows.Forms.ColumnHeader();
            this.colValue    = new System.Windows.Forms.ColumnHeader();
            this.colExpected = new System.Windows.Forms.ColumnHeader();
            this.colStatus   = new System.Windows.Forms.ColumnHeader();

            this.grpInput.SuspendLayout();
            this.SuspendLayout();

            // ── grpInput ──────────────────────────────────────────
            this.grpInput.Dock     = System.Windows.Forms.DockStyle.Top;
            this.grpInput.Font     = defaultFont;
            this.grpInput.Height   = 130;
            this.grpInput.Text     = "輸入 Register 資料";
            this.grpInput.Padding  = new System.Windows.Forms.Padding(6);
            this.grpInput.Controls.AddRange(new System.Windows.Forms.Control[]
                { this.lblHint, this.txtInput, this.btnSample, this.btnParse, this.btnClear, this.lblSummary });

            // lblHint
            this.lblHint.AutoSize = true;
            this.lblHint.Font     = defaultFont;
            this.lblHint.Location = new System.Drawing.Point(8, 20);
            this.lblHint.Text     = "輸入 38 個 Register 值（十六進位，空白或逗號分隔，例：0100 0100 ...）：";

            // txtInput
            this.txtInput.Font       = new System.Drawing.Font("Consolas", 9F);
            this.txtInput.Location   = new System.Drawing.Point(8, 38);
            this.txtInput.Size       = new System.Drawing.Size(900, 22);
            this.txtInput.Anchor     = System.Windows.Forms.AnchorStyles.Left
                                     | System.Windows.Forms.AnchorStyles.Right
                                     | System.Windows.Forms.AnchorStyles.Top;

            // btnSample
            this.btnSample.Font     = defaultFont;
            this.btnSample.Location = new System.Drawing.Point(8, 68);
            this.btnSample.Size     = new System.Drawing.Size(160, 28);
            this.btnSample.Text     = "填入 Sample Data（規格 8.2）";
            this.btnSample.Click   += new System.EventHandler(this.btnSampleData_Click);

            // btnParse
            this.btnParse.Font     = defaultFont;
            this.btnParse.Location = new System.Drawing.Point(176, 68);
            this.btnParse.Size     = new System.Drawing.Size(80, 28);
            this.btnParse.Text     = "解析";
            this.btnParse.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            this.btnParse.ForeColor = System.Drawing.Color.White;
            this.btnParse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnParse.Click   += new System.EventHandler(this.btnParse_Click);

            // btnClear
            this.btnClear.Font     = defaultFont;
            this.btnClear.Location = new System.Drawing.Point(264, 68);
            this.btnClear.Size     = new System.Drawing.Size(75, 28);
            this.btnClear.Text     = "清除";
            this.btnClear.Click   += new System.EventHandler(this.btnClear_Click);

            // lblSummary
            this.lblSummary.AutoSize  = true;
            this.lblSummary.Font      = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Bold);
            this.lblSummary.Location  = new System.Drawing.Point(8, 102);
            this.lblSummary.ForeColor = System.Drawing.Color.DimGray;
            this.lblSummary.Text      = "";

            // ── listResults ───────────────────────────────────────
            this.listResults.Dock         = System.Windows.Forms.DockStyle.Fill;
            this.listResults.Font         = new System.Drawing.Font("Consolas", 9F);
            this.listResults.FullRowSelect = true;
            this.listResults.GridLines    = true;
            this.listResults.View         = System.Windows.Forms.View.Details;
            this.listResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                { this.colField, this.colByte, this.colValue, this.colExpected, this.colStatus });

            this.colField.Text    = "欄位名稱";
            this.colField.Width   = 260;
            this.colByte.Text     = "Byte 位置";
            this.colByte.Width    = 80;
            this.colValue.Text    = "解析值";
            this.colValue.Width   = 120;
            this.colExpected.Text = "規格預期值";
            this.colExpected.Width = 120;
            this.colStatus.Text   = "符合";
            this.colStatus.Width  = 50;

            // ── UserControl ───────────────────────────────────────
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font          = defaultFont;
            this.Controls.Add(this.listResults);
            this.Controls.Add(this.grpInput);
            this.Name = "OCSParserTest";
            this.Size = new System.Drawing.Size(1000, 700);

            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox  grpInput;
        private System.Windows.Forms.Label     lblHint;
        private System.Windows.Forms.TextBox   txtInput;
        private System.Windows.Forms.Button    btnSample;
        private System.Windows.Forms.Button    btnParse;
        private System.Windows.Forms.Button    btnClear;
        private System.Windows.Forms.Label     lblSummary;
        private System.Windows.Forms.ListView  listResults;
        private System.Windows.Forms.ColumnHeader colField;
        private System.Windows.Forms.ColumnHeader colByte;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ColumnHeader colExpected;
        private System.Windows.Forms.ColumnHeader colStatus;
    }
}
