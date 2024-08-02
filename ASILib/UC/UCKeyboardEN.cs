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
    public partial class UCKeyboardEN : UserControl
    {
        private bool mIsCapsLock = false;

        public delegate void KeyInEventHandler(string keyIn);
        public event KeyInEventHandler KeyInEvt;

        public UCKeyboardEN()
        {
            InitializeComponent();
        }

        private void UCKeyboardEN_Load(object sender, EventArgs e)
        {
            
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

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }

        private void setControls(float newx, float newy, Control cons)
        {
            //遍歷窗體中的控制項，重新設置控制項的值
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//獲取控制項的Tag屬性值，並分割後存儲字元串數組
                float a = System.Convert.ToSingle(mytag[0]) * newx;//根據窗體縮放比例確定控制項的值，寬度
                con.Width = (int)a;//寬度
                a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = System.Convert.ToSingle(mytag[2]) * newx;//左邊距離
                con.Left = (int)(a);
                a = System.Convert.ToSingle(mytag[3]) * newy;//上邊緣距離
                con.Top = (int)(a);
                Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }

    }
}
