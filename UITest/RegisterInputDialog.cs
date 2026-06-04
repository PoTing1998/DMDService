using System;
using System.Drawing;
using System.Windows.Forms;

namespace UITest
{
    /// <summary>
    /// 彈出對話框：逐 Register 填寫 OCS 資料，支援 Hex / 十進位雙向編輯。
    /// 確認後將 38 個 Register 值（hex 字串）回傳給呼叫端。
    /// </summary>
    public partial class RegisterInputDialog : Form
    {
        // 每個 Register 的欄位說明
        private static readonly string[] RegDescriptions = new string[]
        {
            /* 0  */ "NbOfPlatforms | Spare1",
            /* 1  */ "PlatformID（2 bytes LE）",
            /* 2  */ "PreArrival | Arrival",
            /* 3  */ "PreDeparture | Departure",
            /* 4  */ "Skip | Hold",
            /* 5  */ "NbJourneyData | Spare2",
            /* 6  */ "[J1] ValidityField | NbCars",
            /* 7  */ "[J1] TrainUnitID（2 bytes LE）",
            /* 8  */ "[J1] ServiceNumber（2 bytes LE）",
            /* 9  */ "[J1] TripNumber（2 bytes LE）",
            /* 10 */ "[J1] DestinationNumber（2 bytes LE）",
            /* 11 */ "[J1] ArrivalTime Byte1-2（4 bytes LE）",
            /* 12 */ "[J1] ArrivalTime Byte3-4",
            /* 13 */ "[J1] DepartureTime Byte1-2（4 bytes LE）",
            /* 14 */ "[J1] DepartureTime Byte3-4",
            /* 15 */ "[J1] DelayAtArrival（2 bytes LE）",
            /* 16 */ "[J1] DelayAtDeparture（2 bytes LE）",
            /* 17 */ "[J1] CancelledTrain | NextTrainWillNotStop",
            /* 18 */ "[J1] TrainEndOfService | TrainWillNotOpenDoor",
            /* 19 */ "[J1] LastTrainOfOpDay | TrainNotInService",
            /* 20 */ "[J1] LineOperationMode | TestTrain",
            /* 21 */ "[J1] TrainDirection | Spare3",
            /* 22 */ "[J2] ValidityField | NbCars",
            /* 23 */ "[J2] TrainUnitID（2 bytes LE）",
            /* 24 */ "[J2] ServiceNumber（2 bytes LE）",
            /* 25 */ "[J2] TripNumber（2 bytes LE）",
            /* 26 */ "[J2] DestinationNumber（2 bytes LE）",
            /* 27 */ "[J2] ArrivalTime Byte1-2（4 bytes LE）",
            /* 28 */ "[J2] ArrivalTime Byte3-4",
            /* 29 */ "[J2] DepartureTime Byte1-2（4 bytes LE）",
            /* 30 */ "[J2] DepartureTime Byte3-4",
            /* 31 */ "[J2] DelayAtArrival（2 bytes LE）",
            /* 32 */ "[J2] DelayAtDeparture（2 bytes LE）",
            /* 33 */ "[J2] CancelledTrain | NextTrainWillNotStop",
            /* 34 */ "[J2] TrainEndOfService | TrainWillNotOpenDoor",
            /* 35 */ "[J2] LastTrainOfOpDay | TrainNotInService",
            /* 36 */ "[J2] LineOperationMode | TestTrain",
            /* 37 */ "[J2] TrainDirection | Spare4",
        };

        private static readonly ushort[] SampleRegisters = new ushort[]
        {
            0x0100, 0x0100, 0x0100, 0x0000, 0x0001, 0x0200,
            0x0102, 0x0100, 0x2B02, 0x6400, 0x0500,
            0x019D, 0x2301, 0x05A8, 0x2301,
            0x1E00, 0x1E00,
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
            0x0102, 0x0200, 0x2C02, 0x6500, 0x0600,
            0x0000, 0x0000, 0x0000, 0x0000,
            0x0000, 0x1400,
            0x0000, 0x0100, 0x0000, 0x0000, 0x0000,
        };

        /// <summary>確認後可讀取此屬性取得 38 個 hex 字串（空白分隔）</summary>
        public string ResultHexString { get; private set; }

        private bool _updatingCell = false;

        public RegisterInputDialog()
        {
            InitializeComponent();
            BuildGrid();
            LoadValues(new ushort[38]);   // 初始全零
        }

        public RegisterInputDialog(ushort[] initialValues) : this()
        {
            if (initialValues != null && initialValues.Length >= 38)
                LoadValues(initialValues);
        }

        // ── Grid 建立 ─────────────────────────────────────────────

        private void BuildGrid()
        {
            for (int i = 0; i < 38; i++)
            {
                grid.Rows.Add(i.ToString(), RegDescriptions[i], "0000", "0");
                // 標頭區與 J1/J2 分界上色
                if (i < 6)
                    grid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
                else if (i < 22)
                    grid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
                else
                    grid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 235);
            }
        }

        private void LoadValues(ushort[] values)
        {
            for (int i = 0; i < 38; i++)
            {
                grid.Rows[i].Cells[2].Value = values[i].ToString("X4");
                grid.Rows[i].Cells[3].Value = values[i].ToString();
            }
        }

        // ── 事件處理 ─────────────────────────────────────────────

        private void btnSample_Click(object sender, EventArgs e)
        {
            LoadValues(SampleRegisters);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadValues(new ushort[38]);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 驗證所有 Hex 欄位格式正確
            for (int i = 0; i < 38; i++)
            {
                string hex = grid.Rows[i].Cells[2].Value?.ToString() ?? "0";
                ushort val;
                if (!TryParseHex(hex, out val))
                {
                    MessageBox.Show($"Register {i} 的 Hex 值「{hex}」格式不正確（請輸入 0000–FFFF）。",
                                    "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    grid.CurrentCell = grid.Rows[i].Cells[2];
                    return;
                }
            }

            // 組成空白分隔的 hex 字串
            var parts = new string[38];
            for (int i = 0; i < 38; i++)
                parts[i] = grid.Rows[i].Cells[2].Value.ToString().PadLeft(4, '0').ToUpper();

            ResultHexString = string.Join(" ", parts);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_updatingCell) return;
            if (e.RowIndex < 0 || e.RowIndex >= 38) return;

            _updatingCell = true;
            try
            {
                int col = e.ColumnIndex;
                string raw = grid.Rows[e.RowIndex].Cells[col].Value?.ToString() ?? "";

                if (col == 2) // 編輯 Hex → 同步十進位
                {
                    ushort val;
                    if (TryParseHex(raw, out val))
                    {
                        grid.Rows[e.RowIndex].Cells[2].Value = val.ToString("X4");
                        grid.Rows[e.RowIndex].Cells[3].Value = val.ToString();
                        grid.Rows[e.RowIndex].Cells[2].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        grid.Rows[e.RowIndex].Cells[2].Style.ForeColor = Color.Red;
                    }
                }
                else if (col == 3) // 編輯十進位 → 同步 Hex
                {
                    ushort val;
                    if (ushort.TryParse(raw, out val))
                    {
                        grid.Rows[e.RowIndex].Cells[2].Value = val.ToString("X4");
                        grid.Rows[e.RowIndex].Cells[3].Value = val.ToString();
                        grid.Rows[e.RowIndex].Cells[3].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        grid.Rows[e.RowIndex].Cells[3].Style.ForeColor = Color.Red;
                    }
                }
            }
            finally
            {
                _updatingCell = false;
            }
        }

        private static bool TryParseHex(string s, out ushort result)
        {
            s = s?.Trim() ?? "";
            if (s.StartsWith("0x") || s.StartsWith("0X")) s = s.Substring(2);
            return ushort.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out result);
        }
    }
}
