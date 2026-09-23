using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASI.Lib.Comm.Socket.Admin;

namespace UITest
{
    /// <summary>
    /// Socket 連線監控：透過 TaskCMFT / TaskDCU 的本機管理通道，
    /// 查看目前連入的 Client、連線/斷線事件，並可強制斷線或發送測試訊息。
    /// </summary>
    public partial class SocketMonitor : UserControl
    {
        private const int MaxEventRows = 2000;

        private class ServerTarget
        {
            public string Name;
            public string Host;
            public int Port;
        }

        private class PollResult
        {
            public ServerTarget Target;
            public SocketServerStatus Status;
            public List<SocketAdminEvent> Events = new List<SocketAdminEvent>();
            public long LatestSeq;
            public string Error;
            public SocketStationReport Stations;
            public string StationError;
        }

        /// <summary>車站列的資料</summary>
        private class StationRowTag
        {
            public string StationId;
            public string Name;
            /// <summary>目前連線，最新的在前</summary>
            public List<string> Endpoints = new List<string>();
        }

        /// <summary>超過此秒數沒收到資料標示為可疑</summary>
        private const int IdleWarnSeconds = 60;

        /// <summary>各 Server 已讀取到的事件序號</summary>
        private readonly Dictionary<string, long> mEventSeq = new Dictionary<string, long>();

        /// <summary>各 Server 的服務啟動時間，用來判斷服務是否重啟過</summary>
        private readonly Dictionary<string, DateTime> mServiceStart = new Dictionary<string, DateTime>();

        private bool mRefreshing = false;

        public SocketMonitor()
        {
            InitializeComponent();
            InitStationGrid();
            InitClientGrid();
            InitEventGrid();

            cboMsgType.Items.AddRange(new object[]
            {
                new MsgTypeItem(0x00, "Heartbeat (0x00)"),
                new MsgTypeItem(0x01, "Ack (0x01)"),
                new MsgTypeItem(0x02, "Command (0x02)"),
                new MsgTypeItem(0x03, "Response (0x03)")
            });
            cboMsgType.SelectedIndex = 2;
            txtJson.Text = "{\r\n  \"JsonObjectName\": \"\"\r\n}";

            refreshTimer.Interval = (int)numInterval.Value * 1000;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                refreshTimer.Enabled = chkAutoRefresh.Checked;
                RefreshAsync();
            }
        }

        #region Grid

        private void InitStationGrid()
        {
            gridStations.Columns.Clear();
            gridStations.Columns.Add("colStation", "車站");
            gridStations.Columns.Add("colName", "站名");
            gridStations.Columns.Add("colIp", "DCU IP");
            gridStations.Columns.Add("colState", "狀態");
            gridStations.Columns.Add("colCount", "連線數");
            gridStations.Columns.Add("colEndpoint", "目前連線");
            gridStations.Columns.Add("colDuration", "已連線");
            gridStations.Columns.Add("colLastRecv", "最後收到資料");
            gridStations.Columns.Add("colIdle", "閒置");
            gridStations.Columns.Add("colLastDisc", "最後斷線");
            gridStations.Columns.Add("colOffline", "離線多久");
            gridStations.Columns.Add("colRx", "收到 Bytes");

            gridStations.Columns["colStation"].FillWeight = 50;
            gridStations.Columns["colName"].FillWeight = 90;
            gridStations.Columns["colState"].FillWeight = 55;
            gridStations.Columns["colCount"].FillWeight = 45;
            gridStations.Columns["colEndpoint"].FillWeight = 110;
            gridStations.Columns["colRx"].FillWeight = 60;
            gridStations.Columns["colState"].DefaultCellStyle.Font =
                new Font(gridStations.Font, FontStyle.Bold);
        }

        private void InitClientGrid()
        {
            gridClients.Columns.Clear();
            gridClients.Columns.Add("colServer", "Server");
            gridClients.Columns.Add("colEndpoint", "Client (IP:Port)");
            gridClients.Columns.Add("colNote", "車站 / 備註");
            gridClients.Columns.Add("colConnectedAt", "連線時間");
            gridClients.Columns.Add("colDuration", "已連線");
            gridClients.Columns.Add("colLastRecv", "最後收到資料");
            gridClients.Columns.Add("colIdle", "閒置");
            gridClients.Columns.Add("colRx", "收到 Bytes");
            gridClients.Columns.Add("colTx", "管理介面送出");

            gridClients.Columns["colServer"].FillWeight = 50;
            gridClients.Columns["colEndpoint"].FillWeight = 110;
            gridClients.Columns["colNote"].FillWeight = 70;
            gridClients.Columns["colRx"].FillWeight = 60;
            gridClients.Columns["colTx"].FillWeight = 60;
        }

        private void InitEventGrid()
        {
            gridEvents.Columns.Clear();
            gridEvents.Columns.Add("colTime", "時間");
            gridEvents.Columns.Add("colServer", "Server");
            gridEvents.Columns.Add("colType", "事件");
            gridEvents.Columns.Add("colEndpoint", "Client");
            gridEvents.Columns.Add("colDetail", "說明");
            gridEvents.Columns["colTime"].Width = 150;
            gridEvents.Columns["colServer"].Width = 60;
            gridEvents.Columns["colType"].Width = 90;
            gridEvents.Columns["colEndpoint"].Width = 150;
            gridEvents.Columns["colDetail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        #endregion

        #region Refresh

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            // 分頁未顯示時不查詢
            if (!Visible) return;
            RefreshAsync();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshAsync();
        }

        private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            refreshTimer.Enabled = chkAutoRefresh.Checked;
        }

        private void numInterval_ValueChanged(object sender, EventArgs e)
        {
            refreshTimer.Interval = (int)numInterval.Value * 1000;
        }

        private List<ServerTarget> GetTargets()
        {
            var list = new List<ServerTarget>();
            string host = txtHost.Text.Trim();
            if (chkCmft.Checked) list.Add(new ServerTarget { Name = "CMFT", Host = host, Port = (int)numCmftPort.Value });
            if (chkDcu.Checked) list.Add(new ServerTarget { Name = "DCU", Host = host, Port = (int)numDcuPort.Value });
            return list;
        }

        private ServerTarget GetTarget(string serverName)
        {
            return GetTargets().FirstOrDefault(t => t.Name == serverName)
                ?? new ServerTarget
                {
                    Name = serverName,
                    Host = txtHost.Text.Trim(),
                    Port = serverName == "CMFT" ? (int)numCmftPort.Value : (int)numDcuPort.Value
                };
        }

        private void RefreshAsync()
        {
            if (mRefreshing) return;
            mRefreshing = true;

            var targets = GetTargets();
            var seqSnapshot = new Dictionary<string, long>(mEventSeq);
            var startSnapshot = new Dictionary<string, DateTime>(mServiceStart);

            Task.Run(() =>
            {
                var results = new List<PollResult>();
                foreach (var target in targets)
                {
                    results.Add(Poll(target, seqSnapshot, startSnapshot));
                }
                return results;
            }).ContinueWith(t =>
            {
                try
                {
                    if (IsDisposed) return;
                    if (t.Exception != null)
                    {
                        AppendLocalEvent("Error", t.Exception.GetBaseException().Message);
                        return;
                    }
                    ApplyResults(targets, t.Result);
                }
                finally
                {
                    mRefreshing = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static PollResult Poll(ServerTarget target, Dictionary<string, long> seqSnapshot, Dictionary<string, DateTime> startSnapshot)
        {
            var result = new PollResult { Target = target };
            try
            {
                var client = new SocketAdminClient(target.Host, target.Port);
                result.Status = client.List();

                long since;
                if (!seqSnapshot.TryGetValue(target.Name, out since)) since = 0;
                // 服務重啟後序號會歸零，改從頭讀取
                DateTime prevStart;
                if (result.Status.LatestSeq < since ||
                    (startSnapshot.TryGetValue(target.Name, out prevStart) && prevStart != result.Status.ServiceStartedAt))
                {
                    since = 0;
                }

                long latest;
                result.Events = client.Events(since, out latest);
                result.LatestSeq = latest;

                if (target.Name == "DCU")
                {
                    try
                    {
                        result.Stations = client.Stations();
                    }
                    catch (Exception ex)
                    {
                        // 舊版 TaskDCU 沒有 stations 指令
                        result.StationError = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return result;
        }

        private void ApplyResults(List<ServerTarget> targets, List<PollResult> results)
        {
            // 狀態列
            UpdateStatusLabel(lblCmftStatus, "CMFT", results.FirstOrDefault(r => r.Target.Name == "CMFT"), chkCmft.Checked);
            UpdateStatusLabel(lblDcuStatus, "DCU", results.FirstOrDefault(r => r.Target.Name == "DCU"), chkDcu.Checked);

            // 事件
            foreach (var r in results.Where(x => x.Status != null))
            {
                DateTime prevStart;
                bool restarted = mServiceStart.TryGetValue(r.Target.Name, out prevStart) &&
                                 prevStart != r.Status.ServiceStartedAt;
                if (restarted)
                {
                    AppendLocalEvent("Info", $"{r.Status.ProcessName} 已重新啟動（{r.Status.ServiceStartedAt:yyyy-MM-dd HH:mm:ss}）", r.Target.Name);
                }
                mServiceStart[r.Target.Name] = r.Status.ServiceStartedAt;

                foreach (var ev in r.Events)
                {
                    AddEventRow(ev.Time, ev.Server, ev.Type, ev.Endpoint, ev.Detail);
                }
                mEventSeq[r.Target.Name] = r.LatestSeq;
            }

            // Client 清單（保留原本選取）
            string selectedKey = GetSelectedKey();
            DateTime now = DateTime.Now;

            gridClients.SuspendLayout();
            gridClients.Rows.Clear();
            int total = 0;
            foreach (var r in results.Where(x => x.Status != null))
            {
                foreach (var c in r.Status.Clients)
                {
                    bool known = c.ConnectedAt > DateTime.MinValue;
                    int idx = gridClients.Rows.Add(
                        r.Target.Name,
                        c.Endpoint,
                        c.Note,
                        known ? c.ConnectedAt.ToString("yyyy-MM-dd HH:mm:ss") : "-",
                        known ? SocketClientTracker.FormatDuration(now - c.ConnectedAt) : "-",
                        c.LastReceivedAt.HasValue ? c.LastReceivedAt.Value.ToString("HH:mm:ss.fff") : "-",
                        c.LastReceivedAt.HasValue ? SocketClientTracker.FormatDuration(now - c.LastReceivedAt.Value) : "-",
                        c.RxBytes.ToString("N0"),
                        c.TxCount);

                    var row = gridClients.Rows[idx];
                    row.Tag = r.Target.Name + "|" + c.Endpoint;

                    // 超過 60 秒沒有收到任何資料，標示為可疑
                    DateTime lastActive = c.LastReceivedAt ?? c.ConnectedAt;
                    if (known && (now - lastActive).TotalSeconds > 60)
                    {
                        row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                    total++;
                }
            }
            gridClients.ResumeLayout();

            RestoreSelection(selectedKey);
            ApplyStations(results.FirstOrDefault(r => r.Target.Name == "DCU"), now);
            lblClientCount.Text = $"目前連線：{total}　（橘色：超過 60 秒未收到資料）";
            UpdateSelectionUi();
        }

        private void ApplyStations(PollResult r, DateTime now)
        {
            string selectedStation = GetSelectedStation()?.StationId;

            gridStations.SuspendLayout();
            gridStations.Rows.Clear();

            if (!chkDcu.Checked)
            {
                SetStationSummary("DCU 車站：未監控（請勾選 TaskDCU 管理埠）", SystemColors.GrayText);
            }
            else if (r == null || r.Error != null)
            {
                SetStationSummary("DCU 車站：無法連到 TaskDCU 管理通道", Color.Firebrick);
            }
            else if (r.Stations == null)
            {
                SetStationSummary("DCU 車站：TaskDCU 不支援車站查詢（請更新 TaskDCU）- " + r.StationError, Color.Firebrick);
            }
            else
            {
                var rep = r.Stations;
                foreach (var st in rep.Stations)
                {
                    var tag = new StationRowTag
                    {
                        StationId = st.StationId,
                        Name = st.Name,
                        Endpoints = st.Connections.Select(c => c.Endpoint).ToList()
                    };
                    var newest = st.Connections.FirstOrDefault();
                    bool known = newest != null && newest.ConnectedAt > DateTime.MinValue;
                    DateTime? lastRecv = st.Connections.Where(c => c.LastReceivedAt.HasValue)
                                                       .Select(c => c.LastReceivedAt).Max();

                    string offlineFor = "-";
                    if (!st.Online)
                    {
                        offlineFor = st.LastDisconnectedAt.HasValue
                            ? SocketClientTracker.FormatDuration(now - st.LastDisconnectedAt.Value)
                            : "服務啟動後未曾連入";
                    }

                    int idx = gridStations.Rows.Add(
                        st.StationId,
                        st.Name,
                        st.Ip,
                        st.Online ? "● 連線中" : "✕ 離線",
                        st.ConnectionCount,
                        newest != null ? newest.Endpoint : "-",
                        known ? SocketClientTracker.FormatDuration(now - newest.ConnectedAt) : "-",
                        lastRecv.HasValue ? lastRecv.Value.ToString("HH:mm:ss") : "-",
                        lastRecv.HasValue ? SocketClientTracker.FormatDuration(now - lastRecv.Value) : "-",
                        st.LastDisconnectedAt.HasValue ? st.LastDisconnectedAt.Value.ToString("MM-dd HH:mm:ss") : "-",
                        offlineFor,
                        st.Connections.Sum(c => c.RxBytes).ToString("N0"));

                    var row = gridStations.Rows[idx];
                    row.Tag = tag;

                    var stateCell = row.Cells["colState"];
                    if (!st.Online)
                    {
                        row.DefaultCellStyle.BackColor = Color.MistyRose;
                        stateCell.Style.ForeColor = Color.Firebrick;
                    }
                    else if (st.ConnectionCount > 1)
                    {
                        // 同一站多條連線，通常是舊連線沒被清掉
                        row.DefaultCellStyle.BackColor = Color.LemonChiffon;
                        stateCell.Style.ForeColor = Color.DarkOrange;
                        row.Cells["colCount"].Style.ForeColor = Color.DarkOrange;
                    }
                    else
                    {
                        stateCell.Style.ForeColor = Color.ForestGreen;
                        DateTime lastActive = lastRecv ?? newest.ConnectedAt;
                        if (known && (now - lastActive).TotalSeconds > IdleWarnSeconds)
                        {
                            row.Cells["colIdle"].Style.ForeColor = Color.DarkOrange;
                            stateCell.Style.ForeColor = Color.DarkOrange;
                        }
                    }
                }

                int online = rep.Stations.Count(x => x.Online);
                int total = rep.Stations.Count;
                string text = $"DCU 車站：{online} / {total} 連線中";
                var offline = rep.Stations.Where(x => !x.Online).Select(x => x.StationId).ToList();
                if (offline.Count > 0) text += $"　離線：{string.Join("、", offline)}";
                var dup = rep.Stations.Where(x => x.ConnectionCount > 1).Select(x => x.StationId).ToList();
                if (dup.Count > 0) text += $"　重複連線：{string.Join("、", dup)}";
                if (rep.UnknownClients.Count > 0)
                    text += $"　未對應車站的連線 {rep.UnknownClients.Count} 條（{string.Join("、", rep.UnknownClients.Select(c => c.Endpoint))}）";
                if (!string.IsNullOrEmpty(rep.DefinitionError))
                    text += $"　（讀取 station_conf 失敗：{rep.DefinitionError}）";
                if (total == 0)
                    text = "DCU 車站：station_conf 沒有設定 dcu_ip 的車站";

                SetStationSummary(text, total > 0 && online == total && dup.Count == 0 ? Color.ForestGreen : Color.Firebrick);
                tabStations.Text = total > 0 ? $"DCU 車站總覽 ({online}/{total})" : "DCU 車站總覽";
            }

            gridStations.ResumeLayout();

            gridStations.ClearSelection();
            if (selectedStation != null)
            {
                foreach (DataGridViewRow row in gridStations.Rows)
                {
                    if (((StationRowTag)row.Tag).StationId == selectedStation)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void SetStationSummary(string text, Color color)
        {
            lblStationSummary.Text = text;
            lblStationSummary.ForeColor = color;
        }

        private void UpdateStatusLabel(Label label, string name, PollResult r, bool enabled)
        {
            if (!enabled)
            {
                label.Text = $"{name}：未監控";
                label.ForeColor = SystemColors.GrayText;
                return;
            }
            if (r == null) return;

            if (r.Error != null)
            {
                label.Text = $"{name}：無法連到管理通道 {r.Target.Host}:{r.Target.Port}（服務未啟動或管理埠不同）- {r.Error}";
                label.ForeColor = Color.Firebrick;
                return;
            }

            var s = r.Status;
            label.Text = $"{name}：{(s.IsOpen ? "監聽中" : "Socket 未開啟")}　{s.ListenConnStr}　連線數 {s.Clients.Count}　服務啟動 {s.ServiceStartedAt:MM-dd HH:mm:ss}";
            label.ForeColor = s.IsOpen ? Color.ForestGreen : Color.DarkOrange;
        }

        private void AddEventRow(DateTime time, string server, string type, string endpoint, string detail)
        {
            gridEvents.Rows.Insert(0, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), server, TranslateType(type), endpoint, detail);
            var row = gridEvents.Rows[0];
            switch (type)
            {
                case "Connected": row.DefaultCellStyle.ForeColor = Color.ForestGreen; break;
                case "Disconnected": row.DefaultCellStyle.ForeColor = Color.DimGray; break;
                case "Kick": row.DefaultCellStyle.ForeColor = Color.Firebrick; break;
                case "Error": row.DefaultCellStyle.ForeColor = Color.Red; break;
                case "Send": row.DefaultCellStyle.ForeColor = Color.RoyalBlue; break;
            }

            while (gridEvents.Rows.Count > MaxEventRows)
            {
                gridEvents.Rows.RemoveAt(gridEvents.Rows.Count - 1);
            }
        }

        private void AppendLocalEvent(string type, string detail, string server = "UI")
        {
            AddEventRow(DateTime.Now, server, type, null, detail);
        }

        private static string TranslateType(string type)
        {
            switch (type)
            {
                case "Connected": return "連線";
                case "Disconnected": return "斷線";
                case "Kick": return "強制斷線";
                case "Send": return "發送測試";
                case "Error": return "錯誤";
                case "Info": return "資訊";
                default: return type;
            }
        }

        private void btnClearEvents_Click(object sender, EventArgs e)
        {
            gridEvents.Rows.Clear();
        }

        #endregion

        #region Selection

        private string GetSelectedKey()
        {
            if (gridClients.SelectedRows.Count == 0) return null;
            return gridClients.SelectedRows[0].Tag as string;
        }

        private StationRowTag GetSelectedStation()
        {
            if (gridStations.SelectedRows.Count == 0) return null;
            return gridStations.SelectedRows[0].Tag as StationRowTag;
        }

        private bool IsStationView
        {
            get { return tabView.SelectedTab == tabStations; }
        }

        /// <summary>
        /// 取得目前選取的發送 / 斷線目標。車站總覽分頁以該站最新的一條連線為目標。
        /// </summary>
        private bool TryGetSelected(out string server, out string endpoint)
        {
            server = null;
            endpoint = null;

            if (IsStationView)
            {
                var st = GetSelectedStation();
                if (st == null || st.Endpoints.Count == 0) return false;
                server = "DCU";
                endpoint = st.Endpoints[0];
                return true;
            }

            string key = GetSelectedKey();
            if (string.IsNullOrEmpty(key)) return false;
            var parts = key.Split(new[] { '|' }, 2);
            server = parts[0];
            endpoint = parts[1];
            return true;
        }

        private void RestoreSelection(string key)
        {
            gridClients.ClearSelection();
            if (key == null) return;
            foreach (DataGridViewRow row in gridClients.Rows)
            {
                if ((row.Tag as string) == key)
                {
                    row.Selected = true;
                    return;
                }
            }
        }

        private void gridClients_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionUi();
        }

        private void UpdateSelectionUi()
        {
            string server, endpoint;
            bool has = TryGetSelected(out server, out endpoint);
            btnKick.Enabled = !IsStationView && has;
            btnSend.Enabled = has;

            var st = IsStationView ? GetSelectedStation() : null;
            btnKickStation.Enabled = st != null && st.Endpoints.Count > 0;

            if (st != null && !has)
                lblTarget.Text = $"目標：{st.StationId} {st.Name}（離線，無法發送）";
            else if (st != null)
                lblTarget.Text = $"目標：{st.StationId} {st.Name}　{endpoint}" + (st.Endpoints.Count > 1 ? "（有多條連線，送往最新的一條）" : "");
            else
                lblTarget.Text = has ? $"目標：[{server}] {endpoint}" : "目標：(未選取連線)";
        }

        private void tabView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectionUi();
        }

        #endregion

        #region Manage

        private void btnKick_Click(object sender, EventArgs e)
        {
            string server, endpoint;
            if (!TryGetSelected(out server, out endpoint)) return;

            if (MessageBox.Show($"確定要強制斷開 [{server}] {endpoint} 的連線？\r\n\r\n對方若有自動重連機制，稍後會再連入。",
                    "強制斷線", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            var target = GetTarget(server);
            RunManage(() => new SocketAdminClient(target.Host, target.Port).Kick(endpoint), $"已斷開 {endpoint}");
        }

        private void btnKickStation_Click(object sender, EventArgs e)
        {
            var st = GetSelectedStation();
            if (st == null || st.Endpoints.Count == 0) return;

            if (MessageBox.Show($"確定要斷開 {st.StationId} {st.Name} 的所有連線（{st.Endpoints.Count} 條）？\r\n\r\n{string.Join("\r\n", st.Endpoints)}\r\n\r\nDCU 若有自動重連機制，稍後會再連入。",
                    "斷開車站連線", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            var target = GetTarget("DCU");
            var endpoints = st.Endpoints.ToList();
            RunManage(() =>
            {
                var client = new SocketAdminClient(target.Host, target.Port);
                var errors = new List<string>();
                foreach (var ep in endpoints)
                {
                    try { client.Kick(ep); }
                    catch (Exception ex) { errors.Add($"{ep}：{ex.Message}"); }
                }
                if (errors.Count > 0) throw new InvalidOperationException(string.Join("\r\n", errors));
            }, $"已斷開 {st.StationId} 的 {endpoints.Count} 條連線");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string server, endpoint;
            if (!TryGetSelected(out server, out endpoint)) return;

            string json = txtJson.Text.Trim();
            if (json.Length > 0)
            {
                try
                {
                    json = Newtonsoft.Json.Linq.JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.None);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("JSON 格式錯誤：" + ex.Message, "發送測試訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int msgType = ((MsgTypeItem)cboMsgType.SelectedItem).Value;
            int msgId = (int)numMsgId.Value;
            var target = GetTarget(server);

            RunManage(() => new SocketAdminClient(target.Host, target.Port).Send(endpoint, msgType, msgId, json),
                $"已送出測試訊息至 {endpoint}，ID={msgId}",
                () =>
                {
                    if (numMsgId.Value < numMsgId.Maximum) numMsgId.Value++;
                });
        }

        private void RunManage(Action action, string successText, Action onSuccess = null)
        {
            btnKick.Enabled = false;
            btnKickStation.Enabled = false;
            btnSend.Enabled = false;

            Task.Run(action).ContinueWith(t =>
            {
                if (IsDisposed) return;
                if (t.Exception != null)
                {
                    string msg = t.Exception.GetBaseException().Message;
                    AppendLocalEvent("Error", msg);
                    MessageBox.Show(msg, "操作失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendLocalEvent("Info", successText);
                    onSuccess?.Invoke();
                }
                UpdateSelectionUi();
                RefreshAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private class MsgTypeItem
        {
            public int Value { get; private set; }
            private readonly string mText;

            public MsgTypeItem(int value, string text)
            {
                Value = value;
                mText = text;
            }

            public override string ToString()
            {
                return mText;
            }
        }

        #endregion
    }
}
