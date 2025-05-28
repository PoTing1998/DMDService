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
        private TaskOCS homeControl;


        public UITest()
        {
            InitializeComponent();
            homeControl = new TaskOCS();


            // 預設載入首頁
            LoadUserControl(homeControl);
        }

        private void LoadUserControl(UserControl control)
        {
            mainPanel.Controls.Clear(); // 清空現有畫面
            control.Dock = DockStyle.Fill; // 填滿區域
            mainPanel.Controls.Add(control); // 加入新畫面
        }


        private void button1_Click(object sender, EventArgs e)
        {
            LoadUserControl(homeControl);
        }
    }

}
