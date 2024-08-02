using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASI.Lib.UC
{
    public partial class UCKeyboardEN_S : UserControl
    {
        private bool mIsCapsLock = false;

        public delegate void KeyInEventHandler(string keyIn);
        public event KeyInEventHandler KeyInEvt;

        public UCKeyboardEN_S()
        {
            InitializeComponent();
        }

        private void buttonKey_Click(object sender, EventArgs e)
        {
            Button oButton = (Button)sender;
            KeyInEvt(oButton.Text);
        }

        private void buttonCapsLock_Click(object sender, EventArgs e)
        {
            foreach (Control oControl in this.Controls)
            {
                if (oControl.GetType() == typeof(System.Windows.Forms.Button))
                {
                    if (mIsCapsLock)
                    {
                        oControl.Text = oControl.AccessibleDescription;
                    }
                    else
                    {
                        oControl.Text = oControl.Tag.ToString();
                    }
                }
            }

            mIsCapsLock = !mIsCapsLock;
        }
    }
}
