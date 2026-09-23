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
        private OCSParserTest ocsParserControl;
        private SocketMonitor socketMonitorControl;

        private bool[] tabLoaded = new bool[6];

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
            ocsParserControl = new OCSParserTest();
            socketMonitorControl = new SocketMonitor();

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
            // 以 TabPage 物件比對，不要用 SelectedIndex。
            // 分頁在 Designer 中的加入順序若被調整，索引就會對不上，
            // 造成點了某個分頁卻載入別的 UserControl（分頁看起來是空白的）。
            var tab = mainTabControl.SelectedTab;

            if (tab == tabTaskOCS)
                LoadControlIntoTab(tabTaskOCS, taskOCSControl, 0);
            else if (tab == tabTaskCMFT)
                LoadControlIntoTab(tabTaskCMFT, taskCMFTControl, 1);
            else if (tab == tabTaskDCU)
                LoadControlIntoTab(tabTaskDCU, taskDCUControl, 2);
            else if (tab == tabSendToDCU)
                LoadControlIntoTab(tabSendToDCU, sendToDCUControl, 3);
            else if (tab == tabOCSParser)
                LoadControlIntoTab(tabOCSParser, ocsParserControl, 4);
            else if (tab == tabSocketMonitor)
                LoadControlIntoTab(tabSocketMonitor, socketMonitorControl, 5);
        }
    }
}
