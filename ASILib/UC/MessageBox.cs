using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASI.Lib.UC
{
    public partial class MessageBox : Form
    {
#pragma warning disable CS0414 // 已指派欄位 'MessageBox.mDialogResult'，但從未使用過其值。
        private DialogResult mDialogResult = DialogResult.None;
#pragma warning restore CS0414 // 已指派欄位 'MessageBox.mDialogResult'，但從未使用過其值。

        public MessageBox()
        {
            InitializeComponent();
        }

        public DialogResult ShowMsg(IWin32Window owner, string text)
        {
            SetButton(MessageBoxButtons.OK);
            this.richTextBox1.Text = text;
            this.Text = "提示視窗";
            return this.ShowDialog(owner);
        }

        public DialogResult ShowMsg(IWin32Window owner, string text, string caption)
        {
            SetButton(MessageBoxButtons.OK);
            this.richTextBox1.Text = text;
            this.Text = caption;
            return this.ShowDialog(owner);
        }

        public DialogResult ShowMsg(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            SetButton(buttons);
            this.richTextBox1.Text = text;
            this.Text = caption;
            return this.ShowDialog(owner);
        }

        private void SetButton(MessageBoxButtons buttons)
        {
            if (buttons == MessageBoxButtons.OK)
            {
                this.buttonOK.Text = "確定";
                this.buttonOK.Visible = true;
                this.buttonOK.Location = new Point(220, 139);
                this.buttonOK.DialogResult = DialogResult.OK;

                this.buttonCancel.Visible = false;
            }
            else if (buttons == MessageBoxButtons.OKCancel)
            {
                this.buttonOK.Text = "確定";
                this.buttonOK.Visible = true;
                this.buttonOK.Location = new Point(120, 139);
                this.buttonOK.DialogResult = DialogResult.OK;

                this.buttonCancel.Text = "取消";
                this.buttonCancel.Visible = true;
                this.buttonCancel.DialogResult = DialogResult.Cancel;
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                this.buttonOK.Text = "是";
                this.buttonOK.Visible = true;
                this.buttonOK.Location = new Point(120, 139);
                this.buttonOK.DialogResult = DialogResult.Yes;

                this.buttonCancel.Text = "否";
                this.buttonCancel.Visible = true;
                this.buttonCancel.DialogResult = DialogResult.No;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
