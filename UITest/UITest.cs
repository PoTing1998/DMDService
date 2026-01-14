using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UITest
{
    public partial class UITest : Form
    {
        private TaskOCS taskOCSControl;
        private TaskCMFT taskCMFTControl;
        private TaskDCU taskDCUControl;

        public UITest()
        {
            InitializeComponent();

            // 初始化所有 UserControl
            taskOCSControl = new TaskOCS();
            taskCMFTControl = new TaskCMFT();
            taskDCUControl = new TaskDCU();

            // 預設載入 TaskOCS
            LoadUserControl(taskOCSControl);
        }

        private void LoadUserControl(UserControl control)
        {
            mainPanel.Controls.Clear(); // 清空現有畫面
            control.Dock = DockStyle.Fill; // 填滿區域
            mainPanel.Controls.Add(control); // 加入新畫面
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadUserControl(taskOCSControl);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadUserControl(taskCMFTControl);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadUserControl(taskDCUControl);
        }
    }

}
