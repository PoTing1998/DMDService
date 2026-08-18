using System;
using System.Drawing;
using System.Windows.Forms;

namespace UITest
{
    /// <summary>
    /// 彈出對話框：顯示即將傳送的 JSON 內容，允許使用者自由修改後再送出。
    /// </summary>
    public class JsonEditDialog : Form
    {
        private TextBox txtJson;
        private Button btnSend;
        private Button btnCancel;

        /// <summary>使用者確認後的 JSON 內容</summary>
        public string JsonText => txtJson.Text;

        public JsonEditDialog(string title, string initialJson)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            Width = 640;
            Height = 480;
            MinimumSize = new Size(400, 300);

            txtJson = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10F),
                Text = FormatJson(initialJson),
                AcceptsTab = true
            };

            var panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnSend = new Button
            {
                Text = "傳送",
                DialogResult = DialogResult.OK,
                Location = new Point(panelButtons.Width - 180, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Width = 80
            };
            btnCancel = new Button
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Location = new Point(panelButtons.Width - 90, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Width = 80
            };

            panelButtons.Controls.Add(btnSend);
            panelButtons.Controls.Add(btnCancel);

            Controls.Add(txtJson);
            Controls.Add(panelButtons);

            AcceptButton = btnSend;
            CancelButton = btnCancel;
        }

        private static string FormatJson(string json)
        {
            try
            {
                var parsed = Newtonsoft.Json.Linq.JToken.Parse(json);
                return parsed.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }
    }
}
