namespace UITest
{
    partial class UITest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabTaskOCS = new System.Windows.Forms.TabPage();
            this.tabTaskCMFT = new System.Windows.Forms.TabPage();
            this.tabTaskDCU = new System.Windows.Forms.TabPage();
            this.tabSendToDCU = new System.Windows.Forms.TabPage();
            this.mainTabControl.SuspendLayout();
            this.SuspendLayout();
            //
            // mainTabControl
            //
            this.mainTabControl.Controls.Add(this.tabTaskOCS);
            this.mainTabControl.Controls.Add(this.tabTaskCMFT);
            this.mainTabControl.Controls.Add(this.tabTaskDCU);
            this.mainTabControl.Controls.Add(this.tabSendToDCU);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.mainTabControl.ItemSize = new System.Drawing.Size(120, 28);
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.Padding = new System.Drawing.Point(12, 4);
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1328, 880);
            this.mainTabControl.TabIndex = 0;
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);
            //
            // tabTaskOCS
            //
            this.tabTaskOCS.Location = new System.Drawing.Point(4, 32);
            this.tabTaskOCS.Name = "tabTaskOCS";
            this.tabTaskOCS.Padding = new System.Windows.Forms.Padding(3);
            this.tabTaskOCS.Size = new System.Drawing.Size(1320, 844);
            this.tabTaskOCS.TabIndex = 0;
            this.tabTaskOCS.Text = "Task OCS";
            this.tabTaskOCS.UseVisualStyleBackColor = true;
            //
            // tabTaskCMFT
            //
            this.tabTaskCMFT.Location = new System.Drawing.Point(4, 32);
            this.tabTaskCMFT.Name = "tabTaskCMFT";
            this.tabTaskCMFT.Padding = new System.Windows.Forms.Padding(3);
            this.tabTaskCMFT.Size = new System.Drawing.Size(1320, 844);
            this.tabTaskCMFT.TabIndex = 1;
            this.tabTaskCMFT.Text = "Task CMFT";
            this.tabTaskCMFT.UseVisualStyleBackColor = true;
            //
            // tabTaskDCU
            //
            this.tabTaskDCU.Location = new System.Drawing.Point(4, 32);
            this.tabTaskDCU.Name = "tabTaskDCU";
            this.tabTaskDCU.Padding = new System.Windows.Forms.Padding(3);
            this.tabTaskDCU.Size = new System.Drawing.Size(1320, 844);
            this.tabTaskDCU.TabIndex = 2;
            this.tabTaskDCU.Text = "Task DCU";
            this.tabTaskDCU.UseVisualStyleBackColor = true;
            //
            // tabSendToDCU
            //
            this.tabSendToDCU.Location = new System.Drawing.Point(4, 32);
            this.tabSendToDCU.Name = "tabSendToDCU";
            this.tabSendToDCU.Padding = new System.Windows.Forms.Padding(3);
            this.tabSendToDCU.Size = new System.Drawing.Size(1320, 844);
            this.tabSendToDCU.TabIndex = 3;
            this.tabSendToDCU.Text = "Send To DCU";
            this.tabSendToDCU.UseVisualStyleBackColor = true;
            //
            // UITest
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1328, 880);
            this.Controls.Add(this.mainTabControl);
            this.Name = "UITest";
            this.Text = "DMD Service Test UI";
            this.mainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabTaskOCS;
        private System.Windows.Forms.TabPage tabTaskCMFT;
        private System.Windows.Forms.TabPage tabTaskDCU;
        private System.Windows.Forms.TabPage tabSendToDCU;
    }
}
