using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASI.Lib.Utility
{
    /// <summary>
    /// 處理所有子控制項位置及大小依父控制項的長寬變動比例改變的物件
    /// </summary>
    public class RWD
    {
        private System.Collections.Generic.Dictionary<string, ControlState> mControlStateDic = new Dictionary<string, ControlState>();

        /// <summary>
        /// 紀錄下所有控制項的初始位置及大小
        /// </summary>
        /// <param name="parentControl"></param>
        public void KeepIntialState(Control parentControl)
        {
            ControlState oControlState = new ControlState();
            oControlState.ControlSize = parentControl.Size;
            oControlState.ControlLocation = parentControl.Location;
            if (!mControlStateDic.ContainsKey(parentControl.Name))
            {
                mControlStateDic.Add(parentControl.Name, oControlState);
            }

            foreach (Control oControl in parentControl.Controls)
            {
                oControlState = new ControlState();
                oControlState.ControlSize = oControl.Size;
                oControlState.ControlLocation = oControl.Location;
                mControlStateDic.Add(oControl.Name, oControlState);
            }
        }

        /// <summary>
        /// 當父控制項大小改變時，所有的子控制項位置及大小將依父控制項的長寬變動比例改變
        /// </summary>
        /// <param name="parentControl"></param>
        public void SizeChange(Control parentControl)
        {
            ControlState oFormInitialControlState = mControlStateDic[parentControl.Name];
            ControlState oInitialControlState = null;

            //先算出Form的長寬變動比例
            float fWidth = (float)parentControl.Width / (float)oFormInitialControlState.ControlSize.Width;
            float fHeight = (float)parentControl.Height / (float)oFormInitialControlState.ControlSize.Height;

            foreach (Control oControl in parentControl.Controls)
            {
                if (this.mControlStateDic != null &&
                    this.mControlStateDic.TryGetValue(oControl.Name, out oInitialControlState))
                {
                    //控制項位置及大小依Form的長寬變動比例改變
                    oControl.Left = (int)(oInitialControlState.ControlLocation.X * fWidth);
                    oControl.Top = (int)(oInitialControlState.ControlLocation.Y * fHeight);
                    oControl.Width = (int)(oInitialControlState.ControlSize.Width * fWidth);
                    oControl.Height = (int)(oInitialControlState.ControlSize.Height * fHeight);
                }
            }
        }

        public class ControlState
        {
            public Point ControlLocation;
            public Size ControlSize;
        }

    }
}
