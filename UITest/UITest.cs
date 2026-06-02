using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DMDService.Services.Interfaces;
using DMDService.Services.Services;

namespace UITest
{
    public partial class UITest : Form
    {
        private TaskOCS taskOCSControl;
        private TaskCMFT taskCMFTControl;
        private TaskDCU taskDCUControl;
        private SendToDCU sendToDCUControl;

        private bool[] tabLoaded = new bool[4];

        public UITest()
        {
            InitializeComponent();

            // Composition Root — DMD Services
            IDmdConnectionService dmdConnectionService = new DmdConnectionService();
            IDmdMessageService dmdMessageService = new DmdMessageService(dmdConnectionService);

            // Composition Root — CMFT Services
            ICmftConnectionService cmftConnectionService = new CmftConnectionService();
            ICmftMessageService cmftMessageService = new CmftMessageService(cmftConnectionService);

            // Composition Root — OCS Services
            IOcsService ocsService = new OcsService();

            taskOCSControl = new TaskOCS(ocsService);
            taskCMFTControl = new TaskCMFT(cmftMessageService);
            taskDCUControl = new TaskDCU();
            sendToDCUControl = new SendToDCU(dmdMessageService);

            // Default: load SendToDCU tab
            mainTabControl.SelectedTab = tabSendToDCU;
            LoadControlIntoTab(tabSendToDCU, sendToDCUControl, 3);
        }

        private void LoadControlIntoTab(TabPage tab, UserControl control, int tabIndex)
        {
            if (tabLoaded[tabIndex]) return;

            control.Dock = DockStyle.Fill;
            tab.Controls.Add(control);
            tabLoaded[tabIndex] = true;
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (mainTabControl.SelectedIndex)
            {
                case 0:
                    LoadControlIntoTab(tabTaskOCS, taskOCSControl, 0);
                    break;
                case 1:
                    LoadControlIntoTab(tabTaskCMFT, taskCMFTControl, 1);
                    break;
                case 2:
                    LoadControlIntoTab(tabTaskDCU, taskDCUControl, 2);
                    break;
                case 3:
                    LoadControlIntoTab(tabSendToDCU, sendToDCUControl, 3);
                    break;
            }
        }
    }
}
