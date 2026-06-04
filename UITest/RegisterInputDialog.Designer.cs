namespace UITest
{
    partial class RegisterInputDialog
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

            this.grid       = new System.Windows.Forms.DataGridView();
            this.colReg     = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesc    = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHex     = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDec     = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnSample  = new System.Windows.Forms.Button();
            this.btnClear   = new System.Windows.Forms.Button();
            this.btnOK      = new System.Windows.Forms.Button();
            this.btnCancel  = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)this.grid).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // ── DataGridView ──────────────────────────────────────
            this.grid.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.grid.Font                  = new System.Drawing.Font("Consolas", 9F);
            this.grid.AllowUserToAddRows    = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.RowHeadersVisible     = false;
            this.grid.AutoSizeRowsMode      = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight   = 26;
            this.grid.RowTemplate.Height    = 24;
            this.grid.SelectionMode         = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grid.EditMode              = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.grid.CellEndEdit          += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEndEdit);
            this.grid.Columns.AddRange(this.colReg, this.colDesc, this.colHex, this.colDec);

            // colReg
            this.colReg.HeaderText  = "Reg#";
            this.colReg.Width       = 48;
            this.colReg.ReadOnly    = true;
            this.colReg.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            // colDesc
            this.colDesc.HeaderText = "欄位說明";
            this.colDesc.Width      = 340;
            this.colDesc.ReadOnly   = true;
            this.colDesc.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei", 8.5F);

            // colHex
            this.colHex.HeaderText  = "Hex 值（可編輯）";
            this.colHex.Width       = 130;
            this.colHex.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colHex.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 10F);

            // colDec
            this.colDec.HeaderText  = "十進位（可編輯）";
            this.colDec.Width       = 130;
            this.colDec.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;

            // ── pnlButtons ────────────────────────────────────────
            this.pnlButtons.Dock   = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height = 44;
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pnlButtons.Controls.AddRange(new System.Windows.Forms.Control[]
                { this.btnSample, this.btnClear, this.btnOK, this.btnCancel });

            // btnSample
            this.btnSample.Font     = defaultFont;
            this.btnSample.Location = new System.Drawing.Point(6, 8);
            this.btnSample.Size     = new System.Drawing.Size(160, 28);
            this.btnSample.Text     = "填入 Sample Data（規格 8.2）";
            this.btnSample.Click   += new System.EventHandler(this.btnSample_Click);

            // btnClear
            this.btnClear.Font     = defaultFont;
            this.btnClear.Location = new System.Drawing.Point(174, 8);
            this.btnClear.Size     = new System.Drawing.Size(75, 28);
            this.btnClear.Text     = "全部清零";
            this.btnClear.Click   += new System.EventHandler(this.btnClear_Click);

            // btnOK
            this.btnOK.Font      = defaultFont;
            this.btnOK.Location  = new System.Drawing.Point(420, 8);
            this.btnOK.Size      = new System.Drawing.Size(80, 28);
            this.btnOK.Text      = "確定";
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Click    += new System.EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.Font     = defaultFont;
            this.btnCancel.Location = new System.Drawing.Point(508, 8);
            this.btnCancel.Size     = new System.Drawing.Size(75, 28);
            this.btnCancel.Text     = "取消";
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            // ── Form ─────────────────────────────────────────────
            this.AutoScaleMode  = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize     = new System.Drawing.Size(700, 760);
            this.Font           = defaultFont;
            this.Text           = "OCS Register 資料輸入";
            this.StartPosition  = System.Windows.Forms.FormStartPosition.CenterParent;
            this.MinimizeBox    = false;
            this.MaximizeBox    = false;
            this.Controls.Add(this.grid);
            this.Controls.Add(this.pnlButtons);

            ((System.ComponentModel.ISupportInitialize)this.grid).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView                 grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn    colReg;
        private System.Windows.Forms.DataGridViewTextBoxColumn    colDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn    colHex;
        private System.Windows.Forms.DataGridViewTextBoxColumn    colDec;
        private System.Windows.Forms.Panel                        pnlButtons;
        private System.Windows.Forms.Button                       btnSample;
        private System.Windows.Forms.Button                       btnClear;
        private System.Windows.Forms.Button                       btnOK;
        private System.Windows.Forms.Button                       btnCancel;
    }
}
