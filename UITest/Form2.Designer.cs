namespace UITest
{
    partial class Form2
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.taskCMFT1 = new UITest.TaskCMFT();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.taskDCU1 = new UITest.TaskDCU();
            this.taskOCS1 = new UITest.TaskOCS();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(888, 770);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.taskCMFT1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(880, 744);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TaskCMFT";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.taskDCU1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(880, 744);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "TaskDCU";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // taskCMFT1
            // 
            this.taskCMFT1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskCMFT1.Location = new System.Drawing.Point(3, 3);
            this.taskCMFT1.Name = "taskCMFT1";
            this.taskCMFT1.Size = new System.Drawing.Size(874, 738);
            this.taskCMFT1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.taskOCS1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(880, 744);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "TaskOCS";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // taskDCU1
            // 
            this.taskDCU1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskDCU1.Location = new System.Drawing.Point(3, 3);
            this.taskDCU1.Name = "taskDCU1";
            this.taskDCU1.Size = new System.Drawing.Size(874, 738);
            this.taskDCU1.TabIndex = 0;
            // 
            // taskOCS1
            // 
            this.taskOCS1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskOCS1.Location = new System.Drawing.Point(0, 0);
            this.taskOCS1.Name = "taskOCS1";
            this.taskOCS1.Size = new System.Drawing.Size(880, 744);
            this.taskOCS1.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 801);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private TaskCMFT taskCMFT1;
        private TaskDCU taskDCU1;
        private System.Windows.Forms.TabPage tabPage3;
        private TaskOCS taskOCS1;
    }
}