using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UITest
{
    public partial class OCSParserTest : UserControl
    {
        // Sample data from OCS-COM-004 Rev.D Section 8.2 (38 ushorts)
        private static readonly ushort[] SampleRegisters = new ushort[]
        {
            0x0100, 0x0100, 0x0100, 0x0000, 0x0001, 0x0200,  // header
            0x0102, 0x0100, 0x2B02, 0x6400, 0x0500,           // journey1 header
            0x019D, 0x2301,                                    // ArrivalTime1
            0x05A8, 0x2301,                                    // DepartureTime1
            0x1E00, 0x1E00,                                    // Delay1
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000,           // status1
            0x0102, 0x0200, 0x2C02, 0x6500, 0x0600,           // journey2 header
            0x0000, 0x0000,                                    // ArrivalTime2
            0x0000, 0x0000,                                    // DepartureTime2
            0x0000, 0x1400,                                    // Delay2
            0x0000, 0x0100, 0x0000, 0x0000, 0x0000,           // status2
        };

        // Expected values for Sample Data verification
        private static readonly Dictionary<string, string> SampleExpected = new Dictionary<string, string>
        {
            { "NumberOfPlatforms",     "1"        },
            { "PlatformID",            "1"        },
            { "PreArrival",            "1"        },
            { "Arrival",               "0"        },
            { "PreDeparture",          "0"        },
            { "Departure",             "0"        },
            { "Skip",                  "0"        },
            { "Hold",                  "1"        },
            { "NumberOfJourneyData",   "2"        },
            { "ValidityField1",        "1"        },
            { "NumberOfCars1",         "2"        },
            { "TrainUnitID1",          "1"        },
            { "ServiceNumber1",        "555"      },
            { "TripNumber1",           "100"      },
            { "DestinationNumber1",    "5"        },
            { "ArrivalTime1",          "19111169" },
            { "DepartureTime1",        "19113989" },
            { "DelayAtArrival1",       "30"       },
            { "DelayAtDeparture1",     "30"       },
            { "ValidityField2",        "1"        },
            { "NumberOfCars2",         "2"        },
            { "TrainUnitID2",          "2"        },
            { "ServiceNumber2",        "556"      },
            { "TripNumber2",           "101"      },
            { "DestinationNumber2",    "6"        },
            { "DelayAtDeparture2",     "20"       },
            { "TrainEndOfService2",    "1"        },
        };

        public OCSParserTest()
        {
            InitializeComponent();
        }

        private void btnSampleData_Click(object sender, EventArgs e)
        {
            // 解析目前 txtInput 裡已有的值（若有的話）作為 Dialog 初始值
            ushort[] current = null;
            try
            {
                var parsed = ParseInput(txtInput.Text);
                if (parsed.Length >= 38) current = parsed;
            }
            catch { }

            using (var dlg = current != null
                             ? new RegisterInputDialog(current)
                             : new RegisterInputDialog())
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    txtInput.Text = dlg.ResultHexString;
                }
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            listResults.Items.Clear();

            ushort[] registers;
            try
            {
                registers = ParseInput(txtInput.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("輸入格式錯誤：" + ex.Message, "解析失敗",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (registers.Length < 38)
            {
                MessageBox.Show($"需要 38 個值，目前只有 {registers.Length} 個。",
                                "資料不足", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var platform = new OCSPlatform();
            try
            {
                platform.UpdateFromUShortArray(registers);
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析失敗：" + ex.Message, "錯誤",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool usingSample = IsSampleData(registers);
            PopulateResults(platform, usingSample);

            lblSummary.Text = usingSample
                ? $"✓ 使用規格 Sample Data — 共 {listResults.Items.Count} 個欄位"
                : $"解析完成 — 共 {listResults.Items.Count} 個欄位";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            listResults.Items.Clear();
            lblSummary.Text = "";
        }

        // ── 工具方法 ─────────────────────────────────────────────

        private ushort[] ParseInput(string text)
        {
            return text
                .Split(new char[] { ' ', ',', '\t', '\r', '\n' },
                       StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToUInt16(s.TrimStart('0', 'x').TrimStart('0', 'X').Length == 0
                                              ? "0"
                                              : s, 16))
                .ToArray();
        }

        private bool IsSampleData(ushort[] registers)
        {
            if (registers.Length < SampleRegisters.Length) return false;
            for (int i = 0; i < SampleRegisters.Length; i++)
                if (registers[i] != SampleRegisters[i]) return false;
            return true;
        }

        private void PopulateResults(OCSPlatform p, bool checkExpected)
        {
            AddSection("── Header ──────────────────────────");
            AddRow("NumberOfPlatforms",  "0",     p.NumberOfPlatforms,    checkExpected);
            AddRow("Spare1",             "1",     p.Spare1,               false);
            AddRow("PlatformID",         "2–3",   p.PlatformID,           checkExpected);
            AddRow("PreArrival",         "4",     p.PreArrival,           checkExpected);
            AddRow("Arrival",            "5",     p.Arrival,              checkExpected);
            AddRow("PreDeparture",       "6",     p.PreDeparture,         checkExpected);
            AddRow("Departure",          "7",     p.Departure,            checkExpected);
            AddRow("Skip",               "8",     p.Skip,                 checkExpected);
            AddRow("Hold",               "9",     p.Hold,                 checkExpected);
            AddRow("NumberOfJourneyData","10",    p.NumberOfJourneyData,  checkExpected);
            AddRow("Spare2",             "11",    p.Spare2,               false);

            AddSection("── Journey 1 ──────────────────────");
            AddRow("ValidityField1",          "12",    p.ValidityField1,          checkExpected);
            AddRow("NumberOfCars1",           "13",    p.NumberOfCars1,           checkExpected);
            AddRow("TrainUnitID1",            "14–15", p.TrainUnitID1,            checkExpected);
            AddRow("ServiceNumber1",          "16–17", p.ServiceNumber1,          checkExpected);
            AddRow("TripNumber1",             "18–19", p.TripNumber1,             checkExpected);
            AddRow("DestinationNumber1",      "20–21", p.DestinationNumber1,      checkExpected);
            AddRow("ArrivalTime1",            "22–25", p.ArrivalTime1,            checkExpected);
            AddRow("DepartureTime1",          "26–29", p.DepartureTime1,          checkExpected);
            AddRow("DelayAtArrival1",         "30–31", p.DelayAtArrival1,         checkExpected);
            AddRow("DelayAtDeparture1",       "32–33", p.DelayAtDeparture1,       checkExpected);
            AddRow("CancelledTrain1",         "34",    p.CancelledTrain1,         checkExpected);
            AddRow("NextTrainWillNotStop1",   "35",    p.NextTrainWillNotStop1,   checkExpected);
            AddRow("TrainEndOfService1",      "36",    p.TrainEndOfService1,      checkExpected);
            AddRow("TrainWillNotOpenDoor1",   "37",    p.TrainWillNotOpenDoor1,   checkExpected);
            AddRow("LastTrainOfTheOperatingDay1","38", p.LastTrainOfTheOperatingDay1, checkExpected);
            AddRow("TrainNotInService1",      "39",    p.TrainNotInService1,      checkExpected);
            AddRow("LineOperationMode1",      "40",    p.LineOperationMode1,      checkExpected);
            AddRow("TestTrain1",              "41",    p.TestTrain1,              checkExpected);
            AddRow("TrainDirection1",         "42",    p.TrainDirection1,         checkExpected);
            AddRow("Spare3",                  "43",    p.Spare3,                  false);

            AddSection("── Journey 2 ──────────────────────");
            AddRow("ValidityField2",          "44",    p.ValidityField2,          checkExpected);
            AddRow("NumberOfCars2",           "45",    p.NumberOfCars2,           checkExpected);
            AddRow("TrainUnitID2",            "46–47", p.TrainUnitID2,            checkExpected);
            AddRow("ServiceNumber2",          "48–49", p.ServiceNumber2,          checkExpected);
            AddRow("TripNumber2",             "50–51", p.TripNumber2,             checkExpected);
            AddRow("DestinationNumber2",      "52–53", p.DestinationNumber2,      checkExpected);
            AddRow("ArrivalTime2",            "54–57", p.ArrivalTime2,            checkExpected);
            AddRow("DepartureTime2",          "58–61", p.DepartureTime2,          checkExpected);
            AddRow("DelayAtArrival2",         "62–63", p.DelayAtArrival2,         checkExpected);
            AddRow("DelayAtDeparture2",       "64–65", p.DelayAtDeparture2,       checkExpected);
            AddRow("CancelledTrain2",         "66",    p.CancelledTrain2,         checkExpected);
            AddRow("NextTrainWillNotStop2",   "67",    p.NextTrainWillNotStop2,   checkExpected);
            AddRow("TrainEndOfService2",      "68",    p.TrainEndOfService2,      checkExpected);
            AddRow("TrainWillNotOpenDoor2",   "69",    p.TrainWillNotOpenDoor2,   checkExpected);
            AddRow("LastTrainOfTheOperatingDay2","70", p.LastTrainOfTheOperatingDay2, checkExpected);
            AddRow("TrainNotInService2",      "71",    p.TrainNotInService2,      checkExpected);
            AddRow("LineOperationMode2",      "72",    p.LineOperationMode2,      checkExpected);
            AddRow("TestTrain2",              "73",    p.TestTrain2,              checkExpected);
            AddRow("TrainDirection2",         "74",    p.TrainDirection2,         checkExpected);
            AddRow("Spare4",                  "75",    p.Spare4,                  false);
        }

        private void AddSection(string title)
        {
            var item = new ListViewItem(title);
            item.SubItems.Add(""); item.SubItems.Add(""); item.SubItems.Add(""); item.SubItems.Add("");
            item.BackColor = Color.FromArgb(230, 235, 245);
            item.Font = new Font(listResults.Font, FontStyle.Bold);
            listResults.Items.Add(item);
        }

        private void AddRow(string field, string bytePos, object value, bool checkExpected)
        {
            string valStr = value.ToString();
            string expected = "";
            string status = "";
            Color rowColor = Color.White;

            if (checkExpected && SampleExpected.TryGetValue(field, out expected))
            {
                bool match = valStr == expected;
                status = match ? "✓" : "✗";
                rowColor = match ? Color.FromArgb(220, 245, 220) : Color.FromArgb(255, 210, 210);
            }

            var item = new ListViewItem(field);
            item.SubItems.Add(bytePos);
            item.SubItems.Add(valStr);
            item.SubItems.Add(expected);
            item.SubItems.Add(status);
            item.BackColor = rowColor;
            listResults.Items.Add(item);
        }
    }
}
